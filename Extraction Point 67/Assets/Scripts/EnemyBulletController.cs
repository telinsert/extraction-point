using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    [Header("Base Settings")]
    public float speed = 15f;
    public float lifetime = 5f;
    public int damageAmount = 5;

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

    
    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy") || other.CompareTag("Bullet"))
        {
            return;
        }

        
        if (isExplosive)
        {
            Explode();
        }
        else
        {
            
            Health victimHealth = other.GetComponent<Health>();
            if (victimHealth != null)
            {
                victimHealth.TakeDamage(damageAmount);
            }
        }

        
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFXAtPosition("Explosion", transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayerMask);
        foreach (Collider hit in colliders)
        {
            Health victimHealth = hit.GetComponent<Health>();
            if (victimHealth != null)
            {
                victimHealth.TakeDamage(explosionDamage);
            }
        }
    }
}
