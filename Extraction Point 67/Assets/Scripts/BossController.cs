using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(Health), typeof(ZombieAI))]
public class BossController : MonoBehaviour
{
    [Header("Boss Configuration")]
    public string bossName = "Boss";

    [System.Serializable]
    public class BossSummonEntry
    {
        public string name;
        public GameObject prefab;
        public float cooldown = 15f;
        public int count = 1;
        [HideInInspector] public float nextSpawnTime;
    }

    [Header("Summoning Abilities")]
    public List<BossSummonEntry> summons;

    [Header("Enrage Ability")]
    public bool canEnrage = true;
    public float enrageThreshold = 0.4f;
    public float enrageSpeedMultiplier = 1.5f;
    public float enrageCooldownMultiplier = 0.6f;
    public Material enrageMaterial;
    private bool isEnraged = false;

    [Header("Audio - SFX")]
    public string spawnSound = "BossSpawn";
    public string abilitySound = "BossAbility";

    
    [Header("Audio - Music")]
    [Tooltip("Name of the music track in AudioManager to play when boss spawns.")]
    public string bossMusicName = "BossPhase1";
    [Tooltip("Name of the music track in AudioManager to play when boss enrages.")]
    public string enrageMusicName = "BossPhase2";

    private Health health;
    private NavMeshAgent agent;
    private Renderer[] renderers;

    void Start()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        renderers = GetComponentsInChildren<Renderer>();

        

        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.InitializeBoss(health, bossName);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(spawnSound);

            AudioManager.Instance.PlayMusic(bossMusicName);
        }

        foreach (var entry in summons)
        {
            entry.nextSpawnTime = Time.time + entry.cooldown;
        }
    }

    void Update()
    {
        if (health.GetCurrentHealth() <= 0) return;

        HandleSummoning();
        HandleEnrage();
    }

    void HandleSummoning()
    {
        foreach (var entry in summons)
        {
            if (entry.prefab != null && Time.time >= entry.nextSpawnTime)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    SpawnMinion(entry.prefab);
                }

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFXAtPosition(abilitySound, transform.position);

                float currentCooldown = entry.cooldown;
                if (isEnraged)
                {
                    currentCooldown *= enrageCooldownMultiplier;
                }

                entry.nextSpawnTime = Time.time + currentCooldown;
            }
        }
    }

    void SpawnMinion(GameObject prefabToSpawn)
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * 6f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 6f, NavMesh.AllAreas))
        {
            Instantiate(prefabToSpawn, hit.position, Quaternion.identity);
        }
    }

    void HandleEnrage()
    {
        if (!canEnrage || isEnraged) return;

        float healthPercent = (float)health.GetCurrentHealth() / health.MaxHealth;

        if (healthPercent <= enrageThreshold)
        {
            isEnraged = true;

            if (agent != null) agent.speed *= enrageSpeedMultiplier;

            if (enrageMaterial != null)
            {
                foreach (var r in renderers) r.material = enrageMaterial;
            }

            if (BossHealthUI.Instance != null)
            {
                BossHealthUI.Instance.EnableEnragedVisuals(bossName);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(spawnSound); 

                AudioManager.Instance.PlayMusic(enrageMusicName);
            }

            Debug.Log($"{bossName} is ENRAGED! Phase 2 Music Started.");
        }
    }

    private void OnDestroy()
    {
        if (BossHealthUI.Instance != null) BossHealthUI.Instance.HideUI();
        if (AudioManager.Instance != null)
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.buildIndex == 0) // Main Menu
            {
                AudioManager.Instance.PlayMusic("MenuTheme");
            }
            else if (activeScene.buildIndex == 1)
            {
                AudioManager.Instance.PlayMusic("ForestTheme");
            }
            else if (activeScene.buildIndex == 2) 
            {
                AudioManager.Instance.PlayMusic("BeachTheme");
            }
            else if (activeScene.buildIndex == 3) 
            {
                AudioManager.Instance.PlayMusic("CityTheme");
            }
        }
    }
}