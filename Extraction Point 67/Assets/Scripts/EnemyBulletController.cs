using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    [Header("Base Settings")]
    public float speed = 15f;
    public float lifetime = 5f;
    public int damageAmount = 5;

    // --- NEW --- Explosion properties that will be set by the zombie who fired it
    [HideInInspector] public bool isExplosive = false;
    [HideInInspector] public int explosionDamage;
    [HideInInspector] public float explosionRadius;
    [HideInInspector] public GameObject explosionEffectPrefab;
    [HideInInspector] public LayerMask damageableLayerMask;


    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with other enemies or downed players
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DownedPlayer"))
        {
            return;
        }

        // --- MODIFIED --- Check if this bullet should explode
        if (isExplosive)
        {
            Explode();
        }
        else // If not explosive, just do normal damage
        {
            // Check if we hit a player (or anything damageable)
            Health victimHealth = collision.gameObject.GetComponent<Health>();
            if (victimHealth != null)
            {
                victimHealth.TakeDamage(damageAmount);
            }
        }

        // Destroy the bullet on any impact
        Destroy(gameObject);
    }

    // --- NEW --- This logic is similar to the zombie's own explosion
    void Explode()
    {
        // 1. Create the visual effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Find all damageable colliders in the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayerMask);
        foreach (Collider hit in colliders)
        {
            // The layer mask ensures we only hit things that should take damage (Players and Enemies)
            Health victimHealth = hit.GetComponent<Health>();
            if (victimHealth != null)
            {
                // Note: The explosion does NOT do direct impact damage, only area damage.
                // This prevents a player from being hit by both at once.
                victimHealth.TakeDamage(explosionDamage);
            }
        }
    }
}
