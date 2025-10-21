// In /Scripts/Player/StatEffectController.cs
using UnityEngine;

[RequireComponent(typeof(PlayerStats), typeof(Health))]
public class StatEffectController : MonoBehaviour
{
    private PlayerStats stats;
    private Health health;

    // We use an accumulator to avoid healing tiny fractions every frame
    private float regenAccumulator = 0f;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        // --- Handle Health Regeneration ---
        HandleHealthRegen();
    }

    private void HandleHealthRegen()
    {
        // If the player has no regen rate, do nothing.
        if (stats.healthRegenRate <= 0) return;

        // Add the amount of health we should have regenerated this frame to our accumulator
        regenAccumulator += stats.healthRegenRate * Time.deltaTime;

        // If the accumulator has built up to 1 or more...
        if (regenAccumulator >= 1f)
        {
            // ...find out how much whole-number health to heal.
            int healthToRestore = Mathf.FloorToInt(regenAccumulator);

            // Heal the player
            health.Heal(healthToRestore);

            // ...and subtract what we just healed from the accumulator.
            regenAccumulator -= healthToRestore;
        }
    }
}
