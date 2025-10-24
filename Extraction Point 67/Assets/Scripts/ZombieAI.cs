using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] players;
    private Animator animator;
    [Header("Damage Settings")]
    public int damageAmount = 10;
    public float attackRange = 1.5f; // Range within which the zombie can attack
    public float attackCooldown = 1.0f; // Time between attacks
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime); // This controls the switch between Idle and Run

           
            if (agent.speed > 0) // Avoid division by zero
            {
                float speedMultiplier = agent.velocity.magnitude / agent.speed;
                animator.SetFloat("RunSpeedMultiplier", speedMultiplier);
            }
        }

        Transform closestPlayer = GetClosestPlayer();
        if (closestPlayer != null)
        {
            agent.SetDestination(closestPlayer.position);

            // Check if the zombie is within attack range
            if (Vector3.Distance(transform.position, closestPlayer.position) <= attackRange)
            {
                TryAttack(closestPlayer);
            }
        }
    }

    Transform GetClosestPlayer()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = player;
                }
            }
        }
        return closest;
    }

    void TryAttack(Transform player)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log($"Zombie attacked {player.name} for {damageAmount} damage.");
            }
            lastAttackTime = Time.time;
        }
    }
}