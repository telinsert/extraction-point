using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged; // Our new event! (current, max)


    void Start()
    {
        currentHealth = maxHealth;
        // Announce the initial health value
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0; // Clamp health so it doesn't go negative
            Die();
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

    }

    void Die()
    {
        // Trigger the OnDeath event
        OnDeath?.Invoke();

        // Handle death (e.g., play animation, destroy object)
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}