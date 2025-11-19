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
    [HideInInspector] public int pierceCount;
    private TrailRenderer trailRenderer; // Reference to the trail component
    private MeshRenderer meshRenderer;   // Reference to the bullet's renderer
    public Material piercingMaterial;
    public string explosionSound = "Explosion";

    [HideInInspector]
    public GameObject sourcePlayer;
    private Rigidbody rb;


    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime); // Clean up misses

        // Activate visuals if the bullet is a piercing one
        if (pierceCount > 0)
        {
            if (trailRenderer != null) trailRenderer.enabled = true;
            if (meshRenderer != null && piercingMaterial != null) meshRenderer.material = piercingMaterial;
        }
        Destroy(gameObject, lifetime);

    }
    void Awake() // Use Awake instead of Start for component fetching
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void OnTriggerEnter(Collider other) // Renamed from OnCollisionEnter
    {
        // 1. Ignore collisions with the player who fired the bullet
        if (other.gameObject == sourcePlayer)
        {
            return;
        }

        // 2. Check if we hit something that can be damaged (enemies, spawners)
        Health health = other.gameObject.GetComponent<Health>();

        if (health != null)
        {
            // --- It's an enemy or spawner ---

            // Apply all damage and status effects (your logic here is great)
            ApplyDamageAndEffects(health, other.gameObject.GetComponent<StatusEffectReceiver>());

            // Decide whether to pierce or be destroyed.
            if (pierceCount > 0)
            {
                pierceCount--; // Use up one pierce and let the bullet continue.
            }
            else
            {
                // We have no pierces left, so destroy the bullet.
                Destroy(gameObject);
            }
        }
        else
        {
            // --- It's something else, like a wall ---
            // If we hit something without health, always destroy the bullet.
            Destroy(gameObject);
            return;
        }
    }

    // --- NEW HELPER METHOD TO KEEP THINGS CLEAN ---
    private void ApplyDamageAndEffects(Health health, StatusEffectReceiver receiver)
    {
        if (ultimateChance > 0 && Random.value <= ultimateChance)
        {
            NukeExplosion();
            Destroy(gameObject); // Special case: nuke destroys the bullet immediately
            return;
        }
        if (voidChance > 0 && Random.value <= voidChance)
        {
            health.TakeDamage(999999);
            if (explosionRadius > 0) Explode();
            Destroy(gameObject); // Special case: void hit destroys the bullet immediately
            return;
        }

        int finalDamage = damageAmount;
        if (critChance > 0 && Random.value <= critChance)
        {
            finalDamage = Mathf.CeilToInt(damageAmount * critDamage);
            AudioManager.Instance.PlaySFXAtPosition("CritHit", transform.position);
        }
        else
        {
            // --- NEW AUDIO ---
            AudioManager.Instance.PlaySFXAtPosition("Hit", transform.position);
        }

        health.TakeDamage(finalDamage);

        if (receiver != null)
        {
            if (fireDamagePerTick > 0) receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
            if (poisonDamagePerTick > 0) receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
        }

        if (explosionRadius > 0 && Random.value <= explosionChance)
        {
            Explode();
        }
    }
    void Explode()
    {
        AudioManager.Instance.PlaySFXAtPosition(explosionSound, transform.position);

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
        AudioManager.Instance.PlaySFXAtPosition("Nuke", transform.position);

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