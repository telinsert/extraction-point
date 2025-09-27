using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f; // Time in seconds before the bullet disappears

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
        // Try to get the Health component from the object we hit
        Health health = collision.gameObject.GetComponent<Health>();

        // If the object has a Health component, deal damage
        if (health != null)
        {
            health.TakeDamage(10); // Deals 10 damage, you can make this a variable
        }

        // Destroy the bullet itself after hitting anything (except the player)
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}