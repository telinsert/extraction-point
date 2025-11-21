
using UnityEngine;
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{

    public int playerNumber = 1;
    public float autoAimRange = 12f;


    public GameObject bulletPrefab;
    public Transform firePoint;

    private PlayerStats stats;
    private CharacterController controller;
    private Animator animator;
    private Vector3 moveDirection;
    private float nextFireTime = 0f;

    [Header("Revive Settings")]
    public float reviveRange = 2.5f;
    private float reviveProgress = 0f;
    private Transform otherPlayer;
    private Health otherPlayerHealth;
    private ReviveUIController reviveUI;
    [Header("Audio")]
    public string shootSoundName = "Shoot";


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();

        reviveUI = Object.FindFirstObjectByType<ReviveUIController>(); 

        
    }

    void Update()
    {
        if (GameManager.Instance.IsGamePaused) return;


        float horizontal = 0f;
        float vertical = 0f;
        if (otherPlayer == null)
        {
            FindTeammate();
            if (otherPlayer == null) return;
        }
        if (playerNumber == 1)
        {
            horizontal = Input.GetAxis("Horizontal"); 
            vertical = Input.GetAxis("Vertical");     
        }
        else 
        {
            horizontal = Input.GetAxis("Horizontal2");
            vertical = Input.GetAxis("Vertical2");
        }

        moveDirection = new Vector3(horizontal, 0, vertical);
        controller.Move(moveDirection * stats.moveSpeed * Time.deltaTime);

        animator.SetFloat("Speed", moveDirection.magnitude);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null && closestDistance <= autoAimRange)
        {
            Vector3 targetDirection = closestEnemy.transform.position - transform.position;
            targetDirection.y = 0;
            if (targetDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(targetDirection);
            }

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / stats.fireRate;
                Fire();
            }
        }
        else
        {
            Vector3 moveDir = new Vector3(moveDirection.x, 0f, moveDirection.z);
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(moveDir.normalized);
            }
        }

        HandleRevive();
    }

    private void HandleRevive()
    {
        if (otherPlayer == null || otherPlayerHealth == null) return;


        if (otherPlayerHealth.GetCurrentHealth() > 0)
        {

            if (reviveProgress != 0f)
            {

                reviveProgress = 0f;
                if (reviveUI != null) reviveUI.HideAllUI();
            }


            return;
        }

        float distance = Vector3.Distance(transform.position, otherPlayer.position);

        if (distance <= reviveRange)
        {

            reviveProgress += Time.deltaTime;

            if (reviveUI != null)
            {
                reviveUI.ShowReviveProgress(reviveProgress, stats.reviveTime);
            }

            if (reviveProgress >= stats.reviveTime)
            {
                otherPlayerHealth.Revive(0.5f);
                reviveProgress = 0f;
                AudioManager.Instance.PlaySFX("Revive");
            }

        }
        else
        {

            if (reviveProgress > 0f)
            {

                reviveProgress = 0f;
                if (reviveUI != null) reviveUI.HideReviveProgress();
            }
        }
    }

    void Fire()
    {

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            AudioManager.Instance.PlaySFXAtPosition(shootSoundName, transform.position);
            BulletController bulletCtrl = bullet.GetComponent<BulletController>();

            if (bulletCtrl != null)
            {
                bulletCtrl.damageAmount = stats.bulletDamage;
                bulletCtrl.speed = stats.bulletSpeed;
                bulletCtrl.critChance = stats.critChance;   
                bulletCtrl.critDamage = stats.critDamage;
                bulletCtrl.voidChance = stats.voidChance;

                bulletCtrl.fireDamagePerTick = stats.fireDamagePerTick;
                bulletCtrl.fireDuration = stats.fireDuration;
                bulletCtrl.poisonDamagePerTick = stats.poisonDamagePerTick;
                bulletCtrl.poisonDuration = stats.poisonDuration;
                bulletCtrl.poisonSlowAmount = stats.poisonSlowAmount;
                bulletCtrl.sourcePlayer = this.gameObject;
                bulletCtrl.explosionChance = stats.explosionChance;
                bulletCtrl.explosionDamage = stats.explosionDamage;
                bulletCtrl.explosionRadius = stats.explosionRadius;
                bulletCtrl.ultimateChance = stats.ultimateChance;
                bulletCtrl.pierceCount = stats.pierceCount;

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            GetComponent<Health>()?.Heal(25);
            AudioManager.Instance.PlaySFX("Heal");
            Destroy(other.gameObject);
        }
    }
    public void SetTeammate(Transform teammate)
    {
       

        otherPlayer = teammate;
        otherPlayerHealth = teammate.GetComponent<Health>();

        
    }
    private void FindTeammate()
    {
        PlayerStats[] allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (PlayerStats player in allPlayers)
        {
            if (player.gameObject != this.gameObject)
            {
                otherPlayer = player.transform;
                otherPlayerHealth = player.GetComponent<Health>();
                
            }
        }
    }
}