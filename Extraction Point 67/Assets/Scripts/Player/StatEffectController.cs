using UnityEngine;

[RequireComponent(typeof(PlayerStats), typeof(Health))]
public class StatEffectController : MonoBehaviour
{
    private PlayerStats stats;
    private Health health;

    private float regenAccumulator = 0f;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        HandleHealthRegen();
    }

    private void HandleHealthRegen()
    {
        if (stats.healthRegenRate <= 0) return;

        regenAccumulator += stats.healthRegenRate * Time.deltaTime;

        if (regenAccumulator >= 1f)
        {
            int healthToRestore = Mathf.FloorToInt(regenAccumulator);

            health.Heal(healthToRestore);

            regenAccumulator -= healthToRestore;
        }
    }
}
