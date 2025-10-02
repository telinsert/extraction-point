using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 10;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }
}
