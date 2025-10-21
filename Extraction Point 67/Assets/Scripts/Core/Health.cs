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
    private Animator animator;

    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        // Try to get the stats component. It's okay if this is null.
        stats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();

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
        // --- MODIFIED LOGIC START ---
        // Check if this object is a player by seeing if it has PlayerStats
        if (stats != null)
        {
            // --- IT'S A PLAYER ---
            // 1. Notify the GameManager that this player is down.
            GameManager.Instance.OnPlayerDowned(stats.playerNumber);
            // 2. Disable player components to stop them from moving or shooting.
            GetComponent<PlayerController>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            gameObject.tag = "DownedPlayer"; // Change the tag so zombies will ignore this player.

            if (animator != null) // Tell the animator to play the downed animation
            {
                animator.SetTrigger("IsDowned");
            }

            Debug.Log($"Player {stats.playerNumber} is downed.");
        }
        else
        {
            // --- IT'S AN ENEMY OR OTHER OBJECT ---
            // Use the original behavior: destroy it.
            Destroy(gameObject);
        }
        // --- MODIFIED LOGIC END ---
    }

    // --- NEW METHOD START ---
    public void Revive(float healthPercentage)
    {
        // 1. Re-enable player components
        GetComponent<PlayerController>().enabled = true;
        GetComponent<CharacterController>().enabled = true;

        // 2. Change the tag back to "Player" so zombies can target them again.
        gameObject.tag = "Player";

        // 3. Heal for a percentage of max health.
        int healthToRestore = Mathf.FloorToInt(MaxHealth * healthPercentage);
        Heal(healthToRestore);

        // 4. Notify the GameManager that this player is back in the fight.
        GameManager.Instance.OnPlayerRevived(stats.playerNumber);

        if (animator != null) // Tell the animator to play the get up animation
        {
            animator.SetTrigger("IsRevived");
        }

        Debug.Log($"Player {stats.playerNumber} has been revived!");
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}