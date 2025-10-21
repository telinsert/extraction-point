using UnityEngine;

// This attribute ensures that a Health component is always on the same GameObject.
// If you try to add this script without a Health component, Unity will add one for you.
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
    private Health spawnerHealth; // This will now be assigned automatically

    void Start()
    {
        // Because of [RequireComponent], we are guaranteed to have a Health component.
        // The logic to add one is no longer needed.
        spawnerHealth = GetComponent<Health>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        InvokeRepeating(nameof(CheckPlayerProximity), 0f, 0.5f);
    }

    // The Update() method is no longer needed as the Health component handles its own death.
    // The OnDeath event is subscribed to in the Die() method of Health.cs.
    // However, we still need a way to trigger our spawner-specific death logic.
    // We do this by listening to the OnDeath event.

    void OnEnable()
    {
        // When the spawner is enabled, subscribe to the death event
        GetComponent<Health>().OnDeath += HandleSpawnerDeath;
    }

    void OnDisable()
    {
        // When the spawner is disabled or destroyed, unsubscribe to prevent memory leaks
        GetComponent<Health>().OnDeath -= HandleSpawnerDeath;
    }

    // This method will be called automatically when the Health component reaches zero.
    void HandleSpawnerDeath()
    {
        Debug.Log($"{gameObject.name} spawner destroyed!");
        if (chestPrefab != null)
        {
            Instantiate(chestPrefab, transform.position + chestoffset, Quaternion.identity);
        }
        // The Health script will handle destroying the GameObject.
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

    
}