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
    HealthRegen
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
        }
    }
}
