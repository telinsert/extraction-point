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
}