// In /Scripts/Core/Health.cs
using UnityEngine;
using System;

// We REMOVED the [RequireComponent(typeof(PlayerStats))] attribute. This is the key change.
public class Health : MonoBehaviour
{
    [Header("Default Health Settings")]
    [SerializeField] private int initialMaxHealth = 100;
    public int MaxHealth { get; private set; }

    private int currentHealth;
    private PlayerStats stats; // This will remain null for non-player objects.

    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        // Try to get the stats component. It's okay if this is null.
        stats = GetComponent<PlayerStats>();

        // If we ARE on a player that has stats...
        if (stats != null)
        {
            // ...then our max health is determined by the stats component.
            MaxHealth = stats.maxHealth;
        }
        else
        {
            // Otherwise, use the default value set in the inspector.
            MaxHealth = initialMaxHealth;
        }
        // If stats is null (because we're on a Zombie), it will just use the maxHealth value set above.
    }

    void Start()
    {
        currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    // This method is now used to update health when a player gets an upgrade.
    public void UpdateMaxHealth(int newMaxHealth, int amountToHeal)
    {
        MaxHealth = newMaxHealth;
        Heal(amountToHeal); // Give the health boost immediately
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}