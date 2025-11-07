// In /Scripts/Enemies/StatusEffectReceiver.cs
// In /Scripts/Enemies/StatusEffectReceiver.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

// A helper class to store data for one instance of an effect
public class ActiveStatusEffect
{
    public int SourceID; // Unique ID of the player who applied it
    public int DamagePerTick;
    public float SlowAmount;
    public float RemainingDuration;
}

public class StatusEffectReceiver : MonoBehaviour
{
    [Header("Particle Effect Prefabs")]
    public GameObject fireParticlesPrefab;
    public GameObject poisonParticlesPrefab;

    // --- Component References ---
    private Health health;
    private NavMeshAgent agent;
    private PlayerController playerController; // Using this for player speed now

    // --- New Data Structures for Stacking ---
    private readonly List<ActiveStatusEffect> activeFireEffects = new List<ActiveStatusEffect>();
    private readonly List<ActiveStatusEffect> activePoisonEffects = new List<ActiveStatusEffect>();

    // --- Manager Coroutines ---
    private Coroutine fireManagerCoroutine;
    private Coroutine poisonManagerCoroutine;

    // --- Speed Management ---
    private float originalAgentSpeed = -1f;
    private float originalPlayerMoveSpeed = -1f;
    private GameObject activeFireParticles;
    private GameObject activePoisonParticles;

    void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        playerController = GetComponent<PlayerController>();
    }

    public void ApplyFire(int damage, float duration, GameObject source)
    {
        ApplyEffect(activeFireEffects, damage, 0f, duration, source, ref fireManagerCoroutine, ManageFireEffects);
    }

    public void ApplyPoison(int damage, float duration, float slow, GameObject source)
    {
        ApplyEffect(activePoisonEffects, damage, slow, duration, source, ref poisonManagerCoroutine, ManagePoisonEffects);
    }

    private void ApplyEffect(List<ActiveStatusEffect> effectList, int damage, float slow, float duration, GameObject source, ref Coroutine managerCoroutine, System.Func<IEnumerator> managerMethod)
    {
        int sourceID = source.GetInstanceID();
        var existingEffect = effectList.FirstOrDefault(e => e.SourceID == sourceID);

        if (existingEffect != null)
        {
            // If the effect from this source already exists, just refresh its duration
            existingEffect.RemainingDuration = duration;
        }
        else
        {
            // Otherwise, add a new instance of the effect to our list
            effectList.Add(new ActiveStatusEffect
            {
                SourceID = sourceID,
                DamagePerTick = damage,
                SlowAmount = slow,
                RemainingDuration = duration
            });
        }

        // If the manager coroutine for this effect type isn't running, start it.
        if (managerCoroutine == null)
        {
            managerCoroutine = StartCoroutine(managerMethod());
        }
    }

    private IEnumerator ManageFireEffects()
    {
        if (activeFireParticles == null && fireParticlesPrefab != null)
        {
            activeFireParticles = Instantiate(fireParticlesPrefab, transform.position, Quaternion.identity, transform);
        }

        // Run this loop as long as there is at least one active fire effect
        while (activeFireEffects.Count > 0)
        {
            yield return new WaitForSeconds(1.0f);

            // 1. Calculate STACKED damage from all active fire sources
            int totalDamageThisTick = activeFireEffects.Sum(e => e.DamagePerTick);
            if (health != null) health.TakeDamage(totalDamageThisTick);

            // 2. Decrease duration for all effects and remove expired ones
            // We loop backwards because we are modifying the list as we iterate
            for (int i = activeFireEffects.Count - 1; i >= 0; i--)
            {
                activeFireEffects[i].RemainingDuration -= 1.0f;
                if (activeFireEffects[i].RemainingDuration <= 0)
                {
                    activeFireEffects.RemoveAt(i);
                }
            }
        }

        // Cleanup
        Destroy(activeFireParticles);
        fireManagerCoroutine = null; // Mark as finished
    }

    private IEnumerator ManagePoisonEffects()
    {
        if (activePoisonParticles == null && poisonParticlesPrefab != null)
        {
            activePoisonParticles = Instantiate(poisonParticlesPrefab, transform.position, Quaternion.identity, transform);
        }

        while (activePoisonEffects.Count > 0)
        {
            // --- First, handle the strongest slow effect immediately ---
            UpdateSlowEffect();

            yield return new WaitForSeconds(1.0f);

            // 1. Calculate STACKED damage
            int totalDamageThisTick = activePoisonEffects.Sum(e => e.DamagePerTick);
            if (health != null) health.TakeDamage(totalDamageThisTick);

            // 2. Decrease duration and remove expired effects
            for (int i = activePoisonEffects.Count - 1; i >= 0; i--)
            {
                activePoisonEffects[i].RemainingDuration -= 1.0f;
                if (activePoisonEffects[i].RemainingDuration <= 0)
                {
                    activePoisonEffects.RemoveAt(i);
                }
            }
            // After removing expired effects, we MUST update the slow again,
            // in case the strongest slow just wore off.
            UpdateSlowEffect();
        }

        // Cleanup
        Destroy(activePoisonParticles);
        RestoreSpeed();
        poisonManagerCoroutine = null; // Mark as finished
    }

    private void UpdateSlowEffect()
    {
        if (activePoisonEffects.Count > 0)
        {
            // Find the HIGHEST slow amount from all active poison effects
            float strongestSlow = activePoisonEffects.Max(e => e.SlowAmount);
            ApplySlow(strongestSlow);
        }
        else
        {
            // If no poison effects remain, restore speed
            RestoreSpeed();
        }
    }

    private void ApplySlow(float slowAmount)
    {
        if (agent != null)
        {
            if (originalAgentSpeed < 0) originalAgentSpeed = agent.speed;
            agent.speed = originalAgentSpeed * (1 - slowAmount);
        }
        if (playerController != null)
        {
            // We get the original speed from PlayerStats but apply the change via the PlayerController
            if (originalPlayerMoveSpeed < 0) originalPlayerMoveSpeed = GetComponent<PlayerStats>().moveSpeed;
            // This is an example of how you might handle it. Your PlayerController may need adjustment.
            // For now, let's assume direct modification for simplicity, though this might get overridden by PlayerController's Update.
            // A better solution would be a public "speedMultiplier" variable on the PlayerController.
            // Let's stick with the PlayerStats modification for now as per the old script.
            GetComponent<PlayerStats>().moveSpeed = originalPlayerMoveSpeed * (1 - slowAmount);
        }
    }

    private void RestoreSpeed()
    {
        if (agent != null && originalAgentSpeed >= 0)
        {
            agent.speed = originalAgentSpeed;
            originalAgentSpeed = -1f;
        }
        if (playerController != null && originalPlayerMoveSpeed >= 0)
        {
            GetComponent<PlayerStats>().moveSpeed = originalPlayerMoveSpeed;
            originalPlayerMoveSpeed = -1f;
        }
    }
}