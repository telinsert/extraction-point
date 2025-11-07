// In /Scripts/Player/PlayerStats.cs

using UnityEngine;
using System.Collections.Generic; 


public class PlayerStats : MonoBehaviour
{
    public int playerNumber = 1;
    public List<Upgrade> appliedUpgrades = new List<Upgrade>();
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat Stats")]
    public float fireRate = 2f;      // Shots per second
    public int bulletDamage = 10;
    public float bulletSpeed = 20f;  // New stat!

    [Header("Advanced Combat")]
    public float critChance = 0.00f; // 0% chance
    public float critDamage = 2f;  // 100% damage
    public float voidChance = 0.00f;
    [Header("Elemental Effects")]
    // Fire
    public int fireDamagePerTick = 0;
    public float fireDuration = 3f;
    // Poison
    public int poisonDamagePerTick = 0;
    public float poisonDuration = 5f;
    public float poisonSlowAmount = 0.3f;

    [Header("Health Stats")]
    public int maxHealth = 100;
    public float healthRegenRate = 0f;



    // This will eventually hold references to StatusEffect ScriptableObjects
    // public List<StatusEffect> activeEffects;

    void Awake()
    {
        // Here you could load saved data if you had a save system,
        // but for now, we just use the default values.
    }

    // A public method to apply an upgrade from the outside
    public void Apply(Upgrade upgrade)
    {
        appliedUpgrades.Add(upgrade);
        upgrade.ApplyUpgrade(this);
    }
}
