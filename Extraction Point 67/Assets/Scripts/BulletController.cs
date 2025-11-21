using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    public int damageAmount = 10;
    public float critChance;
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
    // Make sure your string matches AudioManager
    public string explosionSound = "Explosion";

    [Header("Ultimate Settings")]
    public float ultimateChance;
    public GameObject nukeEffectPrefab;
    [HideInInspector] public int pierceCount;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;
    public Material piercingMaterial;
    [HideInInspector]
    public GameObject sourcePlayer;
    private Rigidbody rb;

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);

        if (pierceCount > 0)
        {
            if (trailRenderer != null) trailRenderer.enabled = true;
            if (meshRenderer != null && piercingMaterial != null) meshRenderer.sharedMaterial = piercingMaterial;

        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Ignore collisions with the player who fired the bullet
        if (other.gameObject == sourcePlayer)
        {
            return;
        }

        // 2. Check if we hit something that can be damaged
        Health health = other.gameObject.GetComponent<Health>();

        if (health != null)
        {
            // Apply damage
            ApplyDamageAndEffects(health, other.gameObject.GetComponent<StatusEffectReceiver>());

            // --- PIERCE LOGIC ---
            if (pierceCount > 0)
            {
                pierceCount--;

                // --- NEW: NERF DAMAGE BY 25% FOR THE NEXT HIT ---
                // Multiplying by 0.75 keeps 75% of the damage (reducing it by 25%)
                damageAmount = Mathf.CeilToInt(damageAmount * 0.75f);

                // Optional: If damage drops to 0, destroy the bullet early
                if (damageAmount <= 0) Destroy(gameObject);
            }
            else
            {
                // No pierces left, destroy
                Destroy(gameObject);
            }
        }
        else
        {
            // Hit a wall or obstacle
            Destroy(gameObject);
            return;
        }
    }

    private void ApplyDamageAndEffects(Health health, StatusEffectReceiver receiver)
    {
        // --- ULTIMATE CHECK ---
        if (ultimateChance > 0 && Random.value <= ultimateChance)
        {
            NukeExplosion();
            Destroy(gameObject);
            return;
        }

        // --- VOID CHECK ---
        if (voidChance > 0 && Random.value <= voidChance)
        {
            // --- NEW: BOSS CHECK ---
            // We check if the enemy has the "BossController" script
            if (health.GetComponent<BossController>() != null)
            {
                // If it is a Boss, deal 10x Damage
                int bossVoidDamage = damageAmount * 10;
                health.TakeDamage(bossVoidDamage);

                // Play a heavy hit sound if you have one, or standard crit sound
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition("CritHit", transform.position);
            }
            else
            {
                // If it is a normal enemy, Instakill
                health.TakeDamage(999999);
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition("VoidHit", transform.position);
            }

            // Void still triggers explosions if applicable
            if (explosionRadius > 0) Explode();

            // Void hits usually consume the bullet (even if piercing)
            Destroy(gameObject);
            return;
        }

        // --- NORMAL DAMAGE CALCULATION ---
        int finalDamage = damageAmount;

        if (critChance > 0 && Random.value <= critChance)
        {
            finalDamage = Mathf.CeilToInt(damageAmount * critDamage);
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition("CritHit", transform.position);
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition("Hit", transform.position);
        }

        health.TakeDamage(finalDamage);

        // --- STATUS EFFECTS ---
        if (receiver != null)
        {
            if (fireDamagePerTick > 0) receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
            if (poisonDamagePerTick > 0) receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
        }

        // --- EXPLOSION CHANCE ---
        if (explosionRadius > 0 && Random.value <= explosionChance)
        {
            Explode();
        }
    }

    // ... (Explode and NukeExplosion methods remain exactly the same) ...
    void Explode()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition(explosionSound, transform.position);

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                StatusEffectReceiver receiver = hit.GetComponent<StatusEffectReceiver>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(explosionDamage);

                    if (receiver != null)
                    {
                        if (fireDamagePerTick > 0) receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
                        if (poisonDamagePerTick > 0) receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
                    }
                }
            }
        }
    }

    void NukeExplosion()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition("Nuke", transform.position);

        if (nukeEffectPrefab != null)
        {
            Instantiate(nukeEffectPrefab, transform.position, Quaternion.identity);
        }

        float nukeRadius = 15f;
        int nukeDamage = 500;

        Collider[] colliders = Physics.OverlapSphere(transform.position, nukeRadius);

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
                        if (fireDamagePerTick > 0) receiver.ApplyFire(fireDamagePerTick, fireDuration, sourcePlayer);
                        if (poisonDamagePerTick > 0) receiver.ApplyPoison(poisonDamagePerTick, poisonDuration, poisonSlowAmount, sourcePlayer);
                    }
                }
            }
        }
    }
}