using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f; // Time in seconds before the bullet disappears
    public int damageAmount = 10;
    public float critChance;         // <-- ADD THIS
    public float critDamage;
    public float voidChance;

    public int fireDamagePerTick;
    public float fireDuration;
    public int poisonDamagePerTick;
    public float poisonDuration;
    public float poisonSlowAmount;
    [HideInInspector]
    public GameObject sourcePlayer;
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
        StatusEffectReceiver receiver = collision.gameObject.GetComponent<StatusEffectReceiver>();

        

        if (health != null)
        {
            // --- START OF CRIT LOGIC ---
            if (receiver != null)
            {
                // Apply Fire if the player has any fire damage
                if (fireDamagePerTick > 0)
                {
                    // --- MODIFIED --- Pass the source player
                    receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
                }
                // Apply Poison if the player has any poison damage
                if (poisonDamagePerTick > 0)
                {
                    // --- MODIFIED --- Pass all new poison stats and the source player
                    receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
                }
            }
            // 1. Start with the base damage.
            if (Random.value <= voidChance)
            {
                Debug.Log("VOID HIT! Obliterated target.");
                health.TakeDamage(999999); // A huge number to guarantee a kill
                Destroy(gameObject); // Destroy the bullet
                return; // Stop any further damage calculation
            }
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