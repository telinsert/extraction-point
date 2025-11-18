// In /Scripts/Core/PlayerStateData.cs (or a new folder like /Scripts/Data)

using System.Collections.Generic;

// This is NOT a MonoBehaviour. It's a simple C# class.
// Its only job is to be a clean container for our data.
public class PlayerStateData
{
    // A copy of all the fields from PlayerStats that we want to save.
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

    // We also store the list of applied upgrades.
    public List<Upgrade> appliedUpgrades;

    // A special "constructor" method to easily create this data object from a PlayerStats component.
    public PlayerStateData(PlayerStats source)
    {
        if (source == null) return;

        // Copy all the values from the source PlayerStats component.
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

        // Importantly, create a NEW list to avoid reference issues.
        this.appliedUpgrades = new List<Upgrade>(source.appliedUpgrades);
    }
}
