
using System.Collections.Generic;


public class PlayerStateData
{
    public float moveSpeed;
    public float fireRate;
    public int bulletDamage;
    public float bulletSpeed;
    public float critChance;
    public float critDamage;
    public float voidChance;
    public int fireDamagePerTick;
    public float fireDuration;
    public int poisonDamagePerTick;
    public float poisonDuration;
    public float poisonSlowAmount;
    public float explosionChance;
    public int explosionDamage;
    public float explosionRadius;
    public float ultimateChance;
    public int maxHealth;
    public float healthRegenRate;
    public float reviveTime;
    public int pierceCount;

    public List<Upgrade> appliedUpgrades;

    public PlayerStateData(PlayerStats source)
    {
        if (source == null) return;

        this.moveSpeed = source.moveSpeed;
        this.fireRate = source.fireRate;
        this.bulletDamage = source.bulletDamage;
        this.bulletSpeed = source.bulletSpeed;
        this.critChance = source.critChance;
        this.critDamage = source.critDamage;
        this.voidChance = source.voidChance;
        this.fireDamagePerTick = source.fireDamagePerTick;
        this.fireDuration = source.fireDuration;
        this.poisonDamagePerTick = source.poisonDamagePerTick;
        this.poisonDuration = source.poisonDuration;
        this.poisonSlowAmount = source.poisonSlowAmount;
        this.explosionChance = source.explosionChance;
        this.explosionDamage = source.explosionDamage;
        this.explosionRadius = source.explosionRadius;
        this.ultimateChance = source.ultimateChance;
        this.maxHealth = source.maxHealth;
        this.healthRegenRate = source.healthRegenRate;
        this.reviveTime = source.reviveTime;
        this.pierceCount = source.pierceCount;

        this.appliedUpgrades = new List<Upgrade>(source.appliedUpgrades);
    }
}
