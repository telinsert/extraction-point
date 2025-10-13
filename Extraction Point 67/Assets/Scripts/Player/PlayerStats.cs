// In /Scripts/Player/PlayerStats.cs

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerNumber = 1;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat Stats")]
    public float fireRate = 2f;      // Shots per second
    public int bulletDamage = 10;
    public float bulletSpeed = 20f;  // New stat!

    [Header("Advanced Combat")]
    public float critChance = 0.05f; // 5% chance
    public float critDamage = 1.5f;  // 150% damage

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
        upgrade.ApplyUpgrade(this);
    }
}
