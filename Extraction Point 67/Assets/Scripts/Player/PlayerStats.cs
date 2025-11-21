
using UnityEngine;
using System.Collections.Generic; 


public class PlayerStats : MonoBehaviour
{
    public int playerNumber = 1;
    public List<Upgrade> appliedUpgrades = new List<Upgrade>();
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat Stats")]
    public float fireRate = 2f;      
    public int bulletDamage = 10;
    public float bulletSpeed = 20f;  

    [Header("Advanced Combat")]
    public float critChance = 0.00f; 
    public float critDamage = 2f;  // 100% damage
    public float voidChance = 0.00f;
    public int pierceCount = 0;
    [Header("Elemental Effects")]
    // Fire
    public int fireDamagePerTick = 0;
    public float fireDuration = 3f;
    // Poison
    public int poisonDamagePerTick = 0;
    public float poisonDuration = 5f;
    public float poisonSlowAmount = 0.3f;

    public float explosionChance = 0.0f;
    public int explosionDamage = 25;
    public float explosionRadius = 0.2f;
    public float ultimateChance = 0.0f;

    [Header("Health Stats")]
    public int maxHealth = 100;
    public float healthRegenRate = 0f;

    [Header("Interaction Stats")]
    [Tooltip("How many seconds it takes to revive a teammate. Lower is better.")]
    public float reviveTime = 10f;





    void Awake()
    {
        // hi
    }

    public void Apply(Upgrade upgrade)
    {
        appliedUpgrades.Add(upgrade);
        upgrade.ApplyUpgrade(this);
    }

    public void ResetStats()
    {
        Debug.Log($"Resetting stats for Player {playerNumber}.");

        moveSpeed = 5f;
        fireRate = 2f;
        bulletDamage = 10;
        bulletSpeed = 20f;
        critChance = 0.00f;
        critDamage = 2f;
        voidChance = 0.00f;
        fireDamagePerTick = 0;
        poisonDamagePerTick = 0;
        maxHealth = 100;
        healthRegenRate = 0f;
        reviveTime = 10f;
        pierceCount = 0;
        appliedUpgrades.Clear();

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.UpdateMaxHealth(maxHealth, maxHealth); 
        }
    }
    public void ApplyStateData(PlayerStateData data)
    {
        if (data == null) return;

        this.moveSpeed = data.moveSpeed;
        this.fireRate = data.fireRate;
        this.bulletDamage = data.bulletDamage;
        this.bulletSpeed = data.bulletSpeed;
        this.critChance = data.critChance;
        this.critDamage = data.critDamage;
        this.voidChance = data.voidChance;
        this.fireDamagePerTick = data.fireDamagePerTick;
        this.fireDuration = data.fireDuration;
        this.poisonDamagePerTick = data.poisonDamagePerTick;
        this.poisonDuration = data.poisonDuration;
        this.poisonSlowAmount = data.poisonSlowAmount;
        this.explosionChance = data.explosionChance;
        this.explosionDamage = data.explosionDamage;
        this.explosionRadius = data.explosionRadius;
        this.ultimateChance = data.ultimateChance;
        this.maxHealth = data.maxHealth;
        this.healthRegenRate = data.healthRegenRate;
        this.reviveTime = data.reviveTime;
        this.pierceCount = data.pierceCount;


        this.appliedUpgrades = new List<Upgrade>(data.appliedUpgrades);

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.UpdateMaxHealth(this.maxHealth, this.maxHealth);
        }
    }
}
