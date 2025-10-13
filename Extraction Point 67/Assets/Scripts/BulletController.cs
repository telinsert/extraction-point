using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f; // Time in seconds before the bullet disappears
    public int damageAmount = 10;
    public float critChance;         // <-- ADD THIS
    public float critDamage;
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
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            // --- START OF CRIT LOGIC ---

            // 1. Start with the base damage.
            int finalDamage = damageAmount;

            // 2. "Roll the dice" for a critical hit.
            // Random.value returns a random float between 0.0 and 1.0.
            if (Random.value <= critChance)
            {
                // It's a critical hit!
                // 3. Calculate the new damage by applying the multiplier.
                // We use Mathf.CeilToInt to round up, ensuring crits always do at least +1 damage.
                finalDamage = Mathf.CeilToInt(damageAmount * critDamage);

                // It's very helpful to have a visual indicator when testing!
                Debug.Log("CRITICAL HIT! Damage: " + finalDamage);
            }

            // 4. Apply the final calculated damage (either base or crit) to the enemy.
            health.TakeDamage(finalDamage);

            // --- END OF CRIT LOGIC ---
        }

        // Destroy the bullet on any impact.
        Destroy(gameObject);
    }
}