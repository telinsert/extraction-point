// In /Scripts/Upgrades/StatUpgrade.cs

using UnityEngine;

// This defines which player stat we are going to modify
public enum StatType
{
    MoveSpeed,
    FireRate,
    BulletDamage,
    BulletSpeed,
    CritChance,
    CritDamage,
    MaxHealth,
    HealthRegen,
    VoidChance,
    FireDamage,
    FireDuration,
    PoisonDamage,
    PoisonDuration,
    PoisonSlow
}

// We add the CreateAssetMenu attribute here, on the specific type, not the base class.
[CreateAssetMenu(fileName = "New Stat Upgrade", menuName = "Roguelike/Stat Upgrade")]
public class StatUpgrade : Upgrade
{
    [Header("Stat Upgrade Settings")]
    public StatType statToModify; // Which stat will this upgrade change?

    public float value;           // How much to change it by?
    public bool isMultiplier = false; // Is this a percentage (e.g., +10%) or a flat value (e.g., +5)?

    // This is the core logic. We override the base class's empty method.
    public override void ApplyUpgrade(PlayerStats targetStats)
    {
        // Use a switch statement to find the correct stat and modify it
        switch (statToModify)
        {
            case StatType.MoveSpeed:
                if (isMultiplier)
                    targetStats.moveSpeed *= (1 + value); // e.g., value = 0.1 for +10%
                else
                    targetStats.moveSpeed += value;
                break;

            case StatType.FireRate:
                if (isMultiplier)
                    targetStats.fireRate *= (1 + value);
                else
                    targetStats.fireRate += value;
                break;

            case StatType.BulletDamage:
                if (isMultiplier)
                    // We must cast to int for damage
                    targetStats.bulletDamage = Mathf.CeilToInt(targetStats.bulletDamage * (1 + value));
                else
                    targetStats.bulletDamage += (int)value;
                break;

            case StatType.BulletSpeed:
                if (isMultiplier)
                    targetStats.bulletSpeed *= (1 + value);
                else
                    targetStats.bulletSpeed += value;
                break;

            case StatType.CritChance:
                // Crit chance is almost always additive
                targetStats.critChance += value; // e.g., value = 0.05 for +5%
                break;

            case StatType.CritDamage:
                if (isMultiplier)
                    targetStats.critDamage *= (value);
                else
                    targetStats.critDamage += value;
                break;
            case StatType.MaxHealth:
                int healthIncrease = (int)value;

                // 1. Permanently increase the stat on the PlayerStats component.
                targetStats.maxHealth += healthIncrease;

                // 2. Find the Health component on the player.
                Health health = targetStats.GetComponent<Health>();
                if (health != null)
                {
                    // 3. Tell the Health component to update itself with the new value.
                    health.UpdateMaxHealth(targetStats.maxHealth, healthIncrease);
                }
                break;
            case StatType.HealthRegen:
                // Regen is a simple additive value
                targetStats.healthRegenRate += value;
                break;
            case StatType.VoidChance:
                if (isMultiplier)
                    targetStats.voidChance *= (value);
                else
                    targetStats.voidChance += value; // Value should be small, like 0.01
                break;
            case StatType.FireDamage:
                if (isMultiplier)
                    targetStats.fireDamagePerTick = Mathf.CeilToInt(targetStats.fireDamagePerTick * (value));
                else
                    targetStats.fireDamagePerTick += (int)value;
                break;
            case StatType.FireDuration:
                targetStats.fireDuration += value;
                break;
            case StatType.PoisonDamage:
                if (isMultiplier)
                    targetStats.poisonDamagePerTick = Mathf.CeilToInt(targetStats.poisonDamagePerTick * (value));
                else
                    targetStats.poisonDamagePerTick += (int)value;
                break;
            case StatType.PoisonDuration:
                targetStats.poisonDuration += value;
                break;
            case StatType.PoisonSlow: // <-- ADD THIS CASE
                                      // Increase the slow amount. Clamp at 90% to prevent total stoppage.
                targetStats.poisonSlowAmount = Mathf.Clamp(targetStats.poisonSlowAmount + value, 0f, 0.9f);
                break;

        }
    }
}
