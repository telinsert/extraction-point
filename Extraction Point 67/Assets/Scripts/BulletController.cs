using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f; // Time in seconds before the bullet disappears
    public int damageAmount = 10;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Make the bullet move forward from the moment it's created
        rb.linearVelocity = transform.forward * speed;

        // Destroy the bullet after a certain time to clean up misses
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet hits something with a Health component
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            // Apply damage directly from the bullet
            health.TakeDamage(damageAmount);
        }

        // Destroy the bullet on any impact to prevent it from piercing
        // You can keep your old tag check if you want bullets to only die on enemies
        Destroy(gameObject);
    }
}