// In /Scripts/Core/Health.cs
using System;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [Header("Default Health Settings")]
#pragma warning disable 0649
    [SerializeField] private int initialMaxHealth = 100;
#pragma warning restore 0649
    public int MaxHealth { get; private set; }

    private int currentHealth;
    private PlayerStats stats;
    private Animator animator;
    private bool isDowned = false;
    [Header("Audio")]
    public string hurtSound = "EnemyHurt";
    public string deathSound = "EnemyDeath";
    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        // --- THIS METHOD IS NOW CORRECTED ---
        stats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();

        if (stats != null) // If this is a player
        {
            MaxHealth = stats.maxHealth;
            hurtSound = "PlayerHurt";
            deathSound = "PlayerDown";
        }
        else // If this is a Zombie or other non-player
        {
            // Restore the original, correct logic:
            MaxHealth = initialMaxHealth;
        }
    }

    void Start()
    {
        currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void UpdateMaxHealth(int newMaxHealth, int amountToHeal)
    {
        MaxHealth = newMaxHealth;
        Heal(amountToHeal);
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDowned) return;

        if (amount >= 10)
            AudioManager.Instance.PlaySFXAtPosition(hurtSound, transform.position);

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
        if (isDowned) return;
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    void Die()
    {
        AudioManager.Instance.PlaySFXAtPosition(deathSound, transform.position);


        OnDeath?.Invoke();

        if (stats != null) // If this is a player
        {
            isDowned = true;
            GameManager.Instance.OnPlayerDowned(stats.playerNumber);
            GetComponent<PlayerController>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            gameObject.tag = "DownedPlayer";

            if (animator != null)
            {
                animator.SetTrigger("IsDowned");
            }
            Debug.Log($"Player {stats.playerNumber} is downed.");
        }
        else // If this is an Enemy
        {
            if (GetComponent<ZombieAI>() != null)
            {
                // It's a ZOMBIE. Use the animation and delay.
                if (animator != null) animator.SetTrigger("Die");
                if (GetComponent<NavMeshAgent>() != null) GetComponent<NavMeshAgent>().enabled = false;
                if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
                GetComponent<ZombieAI>().enabled = false; // Disable the AI script itself.

                Destroy(gameObject, 0.65f);
            }
            else
            {
                // It's a SPAWNER or some other destructible object.
                Destroy(gameObject);
            }

        }
    }

    public void Revive(float healthPercentage)
    {
        isDowned = false;
        GetComponent<PlayerController>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        gameObject.tag = "Player";
        int healthToRestore = Mathf.FloorToInt(MaxHealth * healthPercentage);
        Heal(healthToRestore);
        GameManager.Instance.OnPlayerRevived(stats.playerNumber);

        if (animator != null)
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