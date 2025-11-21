using UnityEngine;


[RequireComponent(typeof(Health))]
public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject zombiePrefab;
    public float spawnInterval = 5f;
    public int maxZombies = 10;
    public Transform[] spawnPoints;

    [Header("Activation Settings")]
    public float activationRange = 20f;
    private Transform[] players;
    private bool isActive = false;
    public Vector3 chestoffset = new Vector3(0, -0.25f, 0);
    [Header("Loot")]
    public GameObject chestPrefab;

    private int currentZombieCount = 0;
    private Health spawnerHealth;

    void Start()
    {
        spawnerHealth = GetComponent<Health>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        InvokeRepeating(nameof(CheckPlayerProximity), 0f, 0.5f);
    }

   

    void OnEnable()
    {
        GetComponent<Health>().OnDeath += HandleSpawnerDeath;
    }

    void OnDisable()
    {
        GetComponent<Health>().OnDeath -= HandleSpawnerDeath;
    }

    void HandleSpawnerDeath()
    {
        Debug.Log($"{gameObject.name} spawner destroyed!");
        if (chestPrefab != null)
        {
            Instantiate(chestPrefab, transform.position + chestoffset, Quaternion.identity);
        }
    }

    void CheckPlayerProximity()
    {
        foreach (Transform player in players)
        {
            if (player != null && Vector3.Distance(transform.position, player.position) <= activationRange)
            {
                if (!isActive)
                {
                    isActive = true;
                    InvokeRepeating(nameof(SpawnZombie), 0f, spawnInterval);
                }
                return; 
            }
        }

        if (isActive)
        {
            isActive = false;
            CancelInvoke(nameof(SpawnZombie));
        }
    }

    void SpawnZombie()
    {
        if (currentZombieCount >= maxZombies) return;

        Vector3 spawnPosition = transform.position;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPosition = randomPoint.position;
        }

        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        currentZombieCount++;

        Health zombieHealth = zombie.GetComponent<Health>();
        if (zombieHealth != null)
        {
            zombieHealth.OnDeath += () => currentZombieCount--;
        }
    }

    
}