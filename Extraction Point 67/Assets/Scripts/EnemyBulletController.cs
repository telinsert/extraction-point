using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;
    public int damageAmount = 5;

    void Start()
    {
        // Use Rigidbody for movement if available
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        // Destroy the bullet after its lifetime expires to clean up the scene
        Destroy(gameObject, lifetime);
    }

    // We use OnCollisionEnter to detect what the bullet hits.
    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with other enemies or downed players
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DownedPlayer"))
        {
            return;
        }

        // Check if we hit an active player
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(damageAmount);
            }
        }

        // Destroy the bullet on any impact
        Destroy(gameObject);
    }
}
