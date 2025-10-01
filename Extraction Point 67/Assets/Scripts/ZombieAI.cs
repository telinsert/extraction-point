using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] players;

    [Header("Damage Settings")]
    public int damageAmount = 10;
    public float attackRange = 1.5f; // Range within which the zombie can attack
    public float attackCooldown = 1.0f; // Time between attacks
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }
    }

    void Update()
    {
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