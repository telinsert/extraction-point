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
    [Header("Explosion Settings")]
    public float explosionChance;
    public int explosionDamage;
    public float explosionRadius;
    public GameObject explosionEffectPrefab;
    [Header("Ultimate Settings")]
    public float ultimateChance;
    public GameObject nukeEffectPrefab;

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
        // Ignore collisions with the player who fired the bullet
        if (collision.gameObject == sourcePlayer)
        {
            return;
        }

        Health health = collision.gameObject.GetComponent<Health>();
        StatusEffectReceiver receiver = collision.gameObject.GetComponent<StatusEffectReceiver>();

        if (health != null)
        {
            if (ultimateChance > 0 && Random.value <= ultimateChance)
            {
                Debug.Log("ULTIMATE TRIGGERED! SINGULARITY DETONATED!");
                NukeExplosion();
                Destroy(gameObject); // Destroy the bullet that triggered it
                return; // Exit immediately, no other effects apply
            }
            // 1. Check for a VOID hit first, as it overrides everything.
            if (voidChance > 0 && Random.value <= voidChance)
            {
                Debug.Log("VOID HIT! Obliterated target.");
                health.TakeDamage(999999);
                // We can still trigger an explosion on a void hit for a cool effect.
                if (explosionRadius > 0) Explode();
                Destroy(gameObject);
                return; // Exit immediately
            }

            // 2. Calculate and apply direct damage (base or crit).
            int finalDamage = damageAmount;
            if (critChance > 0 && Random.value <= critChance)
            {
                finalDamage = Mathf.CeilToInt(damageAmount * critDamage);
                Debug.Log("CRITICAL HIT! Damage: " + finalDamage);
            }
            health.TakeDamage(finalDamage);

            // 3. Apply status effects (Fire/Poison) to the direct target.
            if (receiver != null)
            {
                if (fireDamagePerTick > 0)
                {
                    receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
                }
                if (poisonDamagePerTick > 0)
                {
                    receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
                }
            }

            // 4. Check for and trigger an explosion.
            if (explosionRadius > 0 && Random.value <= explosionChance)
            {
                Explode();
            }
        }

        // Destroy the bullet on any impact (that isn't the owner).
        Destroy(gameObject);
    }
    void Explode()
    {
        // 1. Create the visual effect (unchanged)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Find all colliders (unchanged)
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // 3. Damage AND apply status effects to every enemy in the radius.
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                StatusEffectReceiver receiver = hit.GetComponent<StatusEffectReceiver>();

                if (enemyHealth != null)
                {
                    // Apply the explosion damage (this is the same)
                    enemyHealth.TakeDamage(explosionDamage);

                    if (receiver != null)
                    {
                        // --- MODIFIED ---
                        // Now we pass the source player to the splashed effects
                        if (fireDamagePerTick > 0)
                        {
                            receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
                        }
                        if (poisonDamagePerTick > 0)
                        {
                            // Pass all poison stats, including the new slow amount and the source player
                            receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
                        }
                    }
                }
            }
        }
    }
    void NukeExplosion()
    {
        // 1. Create a massive visual effect.
        if (nukeEffectPrefab != null)
        {
            Instantiate(nukeEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Define the nuke's properties (hardcoded for maximum impact)
        float nukeRadius = 15f; // A huge radius
        int nukeDamage = 500;   // Massive damage

        // 3. Find all enemies in the massive radius.
        Collider[] colliders = Physics.OverlapSphere(transform.position, nukeRadius);

        // 4. Annihilate them. This is a pure damage event, it does not spread status effects.
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                StatusEffectReceiver receiver = hit.GetComponent<StatusEffectReceiver>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(nukeDamage);

                    if (receiver != null)
                    {
                        if (fireDamagePerTick > 0)
                        {
                            receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
                        }
                        if (poisonDamagePerTick > 0)
                        {
                            receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
                        }
                    }
                }
            }
        }
    }
}