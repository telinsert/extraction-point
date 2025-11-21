
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ActiveStatusEffect
{
    public int SourceID; 
    public int DamagePerTick;
    public float SlowAmount;
    public float RemainingDuration;
}

public class StatusEffectReceiver : MonoBehaviour
{
    [Header("Particle Effect Prefabs")]
    public GameObject fireParticlesPrefab;
    public GameObject poisonParticlesPrefab;

    private Health health;
    private NavMeshAgent agent;
    private PlayerController playerController; 

    private readonly List<ActiveStatusEffect> activeFireEffects = new List<ActiveStatusEffect>();
    private readonly List<ActiveStatusEffect> activePoisonEffects = new List<ActiveStatusEffect>();

    private Coroutine fireManagerCoroutine;
    private Coroutine poisonManagerCoroutine;

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
            existingEffect.RemainingDuration = duration;
        }
        else
        {
            effectList.Add(new ActiveStatusEffect
            {
                SourceID = sourceID,
                DamagePerTick = damage,
                SlowAmount = slow,
                RemainingDuration = duration
            });
        }

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

        while (activeFireEffects.Count > 0)
        {
            yield return new WaitForSeconds(1.0f);

            int totalDamageThisTick = activeFireEffects.Sum(e => e.DamagePerTick);
            if (health != null) health.TakeDamage(totalDamageThisTick);

            for (int i = activeFireEffects.Count - 1; i >= 0; i--)
            {
                activeFireEffects[i].RemainingDuration -= 1.0f;
                if (activeFireEffects[i].RemainingDuration <= 0)
                {
                    activeFireEffects.RemoveAt(i);
                }
            }
        }

        
        Destroy(activeFireParticles);
        fireManagerCoroutine = null; 
    }

    private IEnumerator ManagePoisonEffects()
    {
        if (activePoisonParticles == null && poisonParticlesPrefab != null)
        {
            activePoisonParticles = Instantiate(poisonParticlesPrefab, transform.position, Quaternion.identity, transform);
        }

        while (activePoisonEffects.Count > 0)
        {
            UpdateSlowEffect();

            yield return new WaitForSeconds(1.0f);

            int totalDamageThisTick = activePoisonEffects.Sum(e => e.DamagePerTick);
            if (health != null) health.TakeDamage(totalDamageThisTick);

            for (int i = activePoisonEffects.Count - 1; i >= 0; i--)
            {
                activePoisonEffects[i].RemainingDuration -= 1.0f;
                if (activePoisonEffects[i].RemainingDuration <= 0)
                {
                    activePoisonEffects.RemoveAt(i);
                }
            }
            
            UpdateSlowEffect();
        }

        
        Destroy(activePoisonParticles);
        RestoreSpeed();
        poisonManagerCoroutine = null; 
    }

    private void UpdateSlowEffect()
    {
        if (activePoisonEffects.Count > 0)
        {
            float strongestSlow = activePoisonEffects.Max(e => e.SlowAmount);
            ApplySlow(strongestSlow);
        }
        else
        {
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
            if (originalPlayerMoveSpeed < 0) originalPlayerMoveSpeed = GetComponent<PlayerStats>().moveSpeed;
            
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