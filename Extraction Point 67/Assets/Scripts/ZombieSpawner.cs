using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject zombiePrefab; // Assign the zombie prefab here
    public float spawnInterval = 5f; // Time between spawns
    public int maxZombies = 10; // Maximum zombies spawned at a time
    public Transform[] spawnPoints; // Optional: Specific points to spawn zombies

    [Header("Activation Settings")]
    public float activationRange = 20f; // Range within which the spawner activates
    private Transform[] players; // References to all player transforms
    private bool isActive = false; // Whether the spawner is currently active

    private int currentZombieCount = 0;
    private Health spawnerHealth;

    void Start()
    {
        // Attach a Health component to the spawner if it doesn't already have one
        spawnerHealth = GetComponent<Health>();
        if (spawnerHealth == null)
        {
            spawnerHealth = gameObject.AddComponent<Health>();
            spawnerHealth.maxHealth = 100; // Set default health for the spawner
        }

        // Find all players in the scene
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        // Start checking for player proximity
        InvokeRepeating(nameof(CheckPlayerProximity), 0f, 0.5f); // Check every 0.5 seconds
    }

    void Update()
    {
        // Check if the spawner is destroyed
        if (spawnerHealth.GetCurrentHealth() <= 0)
        {
            DestroySpawner();
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
                return; // Exit the loop if any player is within range
            }
        }

        // If no players are within range, deactivate the spawner
        if (isActive)
        {
            isActive = false;
            CancelInvoke(nameof(SpawnZombie));
        }
    }

    void SpawnZombie()
    {
        if (currentZombieCount >= maxZombies) return;

        // Choose a random spawn point if available
        Vector3 spawnPosition = transform.position;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPosition = randomPoint.position;
        }

        // Spawn the zombie
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        currentZombieCount++;

        // Subscribe to the zombie's OnDeath event to decrement the zombie count
        Health zombieHealth = zombie.GetComponent<Health>();
        if (zombieHealth != null)
        {
            zombieHealth.OnDeath += () => currentZombieCount--;
        }
    }

    public void DestroySpawner()
    {
        // Optional: Add visual effects or animations for destruction
        Debug.Log($"{gameObject.name} spawner destroyed!");
        Destroy(gameObject);
    }
}