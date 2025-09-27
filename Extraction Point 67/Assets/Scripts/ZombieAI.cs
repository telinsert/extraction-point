using UnityEngine;
using UnityEngine.AI; // IMPORTANT: Add this line to use the navigation system

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] players;

    void Start()
    {
        // Get the NavMeshAgent component attached to this zombie
        agent = GetComponent<NavMeshAgent>();

        // Find all GameObjects tagged as "Player" at the start of the game
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }
    }

    void Update()
    {
        // Find the closest player and set them as the destination
        Transform closestPlayer = GetClosestPlayer();
        if (closestPlayer != null)
        {
            agent.SetDestination(closestPlayer.position);
        }
    }

    Transform GetClosestPlayer()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            // Make sure the player object still exists before checking its distance
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

    // This is your existing code for damaging the player on contact
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>()?.TakeDamage(5);
        }
    }
}