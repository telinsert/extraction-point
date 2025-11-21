
using UnityEngine;

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
    PoisonSlow,
    ExplosionChance,
    ExplosionDamage,
    ExplosionRadius,
    UltimateChance,
    ReviveTime,
    PierceCount
}

[CreateAssetMenu(fileName = "New Stat Upgrade", menuName = "Roguelike/Stat Upgrade")]
public class StatUpgrade : Upgrade
{
    [Header("Stat Upgrade Settings")]
    public StatType statToModify; 

    public float value;           
    public bool isMultiplier = false; 

    public override void ApplyUpgrade(PlayerStats targetStats)
    {
        switch (statToModify)
        {
            case StatType.MoveSpeed:
                if (isMultiplier)
                    targetStats.moveSpeed *= (value); 
                else
                    targetStats.moveSpeed += value;
                break;

            case StatType.FireRate:
                if (isMultiplier)
                    targetStats.fireRate *= (value);
                else
                    targetStats.fireRate += value;
                break;

            case StatType.BulletDamage:
                if (isMultiplier)
                    targetStats.bulletDamage = Mathf.CeilToInt(targetStats.bulletDamage * (value));
                else
                    targetStats.bulletDamage += (int)value;
                break;

            case StatType.BulletSpeed:
                if (isMultiplier)
                    targetStats.bulletSpeed *= (value);
                else
                    targetStats.bulletSpeed += value;
                break;

            case StatType.CritChance:
                targetStats.critChance += value; 
                break;

            case StatType.CritDamage:
                if (isMultiplier)
                    targetStats.critDamage *= (value);
                else
                    targetStats.critDamage += value;
                break;
            case StatType.MaxHealth:
                int healthIncrease = (int)value;

                targetStats.maxHealth += healthIncrease;

                Health health = targetStats.GetComponent<Health>();
                if (health != null)
                {
                    health.UpdateMaxHealth(targetStats.maxHealth, healthIncrease);
                }
                break;
            case StatType.HealthRegen:
                targetStats.healthRegenRate += value;
                break;
            case StatType.VoidChance:
                if (isMultiplier)
                    targetStats.voidChance *= (value);
                else
                    targetStats.voidChance += value; 
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
            case StatType.PoisonSlow: 
                targetStats.poisonSlowAmount = Mathf.Clamp(targetStats.poisonSlowAmount + value, 0f, 0.9f);
                break;
            case StatType.ExplosionChance:
                targetStats.explosionChance += value; 
                break;
            case StatType.ExplosionDamage:
                if (isMultiplier)
                    targetStats.explosionDamage = Mathf.CeilToInt(targetStats.explosionDamage * (value));
                else
                    targetStats.explosionDamage += (int)value;
                break;
            case StatType.ExplosionRadius:
                targetStats.explosionRadius += value;
                break;
            case StatType.UltimateChance:
                targetStats.ultimateChance += value;
                break;
            case StatType.ReviveTime:
                if (isMultiplier)
                {
                    targetStats.reviveTime *= (1 - value);
                }
                else
                {
                    targetStats.reviveTime -= value;
                }
                targetStats.reviveTime = Mathf.Max(1f, targetStats.reviveTime);
                break;
            case StatType.PierceCount:
                targetStats.pierceCount += (int)value;
                break;

        }
    }
}
