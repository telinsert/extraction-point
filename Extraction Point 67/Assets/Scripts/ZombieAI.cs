using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] players;
    private Animator animator;
    private Health health;
    private bool isExploding = false;

    [Header("Base Melee Attack")]
    public int meleeDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.0f;
    private float lastAttackTime;

    [Header("Status Effect (On Melee Hit)")]
    public bool appliesFire = false;
    public bool appliesPoison = false;
    public int statusDamagePerTick = 2;
    public float statusDuration = 4f;
    public float poisonSlowAmount = 0.3f;

    [Header("Explosive Behavior (Zombie Itself)")]
    public bool explodesOnDeath = false;
    public bool explodesOnProximity = false;
    public float detonationRange = 2.5f;
    public int explosionDamage = 30;
    public float explosionRadius = 5f;
    public GameObject explosionEffectPrefab;
    public LayerMask damageableLayerMask;

    [Header("Ranged Attack Behavior")]
    public bool canShoot = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootRange = 15f;
    public float shootCooldown = 3.0f;
    private float lastShootTime;

    [Header("Explosive Bullet Settings")]
    public bool hasExplosiveBullets = false;
    public int bulletExplosionDamage = 15;
    public float bulletExplosionRadius = 3f;


    private Transform currentTarget;
    private float targetUpdateInterval = 0.5f;
    private float timeSinceLastTargetUpdate = 0f;
    private float sqrAttackRange;
    private float sqrShootRange;
    private float sqrDetonationRange;


    void OnEnable()
    {
        if (explodesOnDeath)
        {
            health = GetComponent<Health>();
            health.OnDeath += HandleDeathExplosion;
        }
    }

    void OnDisable()
    {
        if (explodesOnDeath && health != null)
        {
            health.OnDeath -= HandleDeathExplosion;
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (health == null) health = GetComponent<Health>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        lastAttackTime = -attackCooldown;
        lastShootTime = -shootCooldown;

        sqrAttackRange = attackRange * attackRange;
        sqrShootRange = shootRange * shootRange;
        sqrDetonationRange = detonationRange * detonationRange;
    }

    void Update()
    {
        if (isExploding) return;

        timeSinceLastTargetUpdate += Time.deltaTime;
        if (timeSinceLastTargetUpdate >= targetUpdateInterval)
        {
            timeSinceLastTargetUpdate = 0f;
            UpdateClosestPlayerTarget();
        }

        if (currentTarget == null)
        {
            if (agent.isOnNavMesh && agent.hasPath) agent.isStopped = true;
            return;
        }

        if (agent.isOnNavMesh) agent.isStopped = false;

        float sqrDistanceToPlayer = (transform.position - currentTarget.position).sqrMagnitude;

        if (explodesOnProximity && sqrDistanceToPlayer <= sqrDetonationRange)
        {
            ExplodeAndDie();
            return;
        }

        if (canShoot && sqrDistanceToPlayer <= sqrShootRange && sqrDistanceToPlayer > sqrAttackRange)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            FaceTarget(currentTarget.position);

            if (Time.time >= lastShootTime + shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
        else if (sqrDistanceToPlayer <= sqrAttackRange)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            FaceTarget(currentTarget.position);
            TryMeleeAttack(currentTarget);
        }
        else
        {
            if (agent.isOnNavMesh && agent.destination != currentTarget.position)
            {
                agent.SetDestination(currentTarget.position);
            }
        }

        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        if (animator != null && agent != null && agent.isOnNavMesh)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

            if (agent.speed > 0)
            {
                float speedMultiplier = agent.velocity.magnitude / agent.speed;
                animator.SetFloat("RunSpeedMultiplier", speedMultiplier);
            }
        }
    }

    void UpdateClosestPlayerTarget()
    {
        Transform closest = null;
        float minSqrDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            if (player != null && player.gameObject.tag != "DownedPlayer")
            {
                float sqrDistance = (transform.position - player.position).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closest = player;
                }
            }
        }
        currentTarget = closest;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void TryMeleeAttack(Transform player)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            if (animator != null) animator.SetTrigger("Attack");
            Health playerHealth = player.GetComponent<Health>();
            StatusEffectReceiver playerStatus = player.GetComponent<StatusEffectReceiver>();
            if (playerHealth != null) playerHealth.TakeDamage(meleeDamage);
            if (playerStatus != null)
            {
                if (appliesFire) playerStatus.ApplyFire(statusDamagePerTick, statusDuration, gameObject);
                if (appliesPoison) playerStatus.ApplyPoison(statusDamagePerTick, statusDuration, poisonSlowAmount, gameObject);
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bulletGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        EnemyBulletController bulletCtrl = bulletGO.GetComponent<EnemyBulletController>();

        if (bulletCtrl != null)
        {
            if (hasExplosiveBullets)
            {
                bulletCtrl.isExplosive = true;
                bulletCtrl.explosionDamage = this.bulletExplosionDamage;
                bulletCtrl.explosionRadius = this.bulletExplosionRadius;
                bulletCtrl.explosionEffectPrefab = this.explosionEffectPrefab;
                bulletCtrl.damageableLayerMask = this.damageableLayerMask;
            }
        }
    }

    void HandleDeathExplosion()
    {
        if (isExploding) return;
        isExploding = true;
        TriggerExplosionVFX();
        DealExplosionDamage();
    }

    void ExplodeAndDie()
    {
        if (isExploding) return;
        isExploding = true;
        TriggerExplosionVFX();
        DealExplosionDamage();
        if (health != null)
        {
            health.TakeDamage(health.GetCurrentHealth() + 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void TriggerExplosionVFX()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void DealExplosionDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayerMask);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject == this.gameObject) continue;

            Health victimHealth = hit.GetComponent<Health>();
            if (victimHealth != null)
            {
                victimHealth.TakeDamage(explosionDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        if (explodesOnProximity)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, detonationRange);
        }
    }
}