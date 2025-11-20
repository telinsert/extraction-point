using UnityEngine;

public class BossSpawnTrigger : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Drag your Boss Prefab here.")]
    public GameObject bossPrefab;

    [Tooltip("Drag an empty GameObject here to define WHERE the boss spawns.")]
    public Transform spawnPoint;

    [Tooltip("Optional: Drag a particle effect here (like smoke or a summoning circle) to play when the boss appears.")]
    public GameObject spawnVFX;

    private bool hasSpawned = false;

    void OnTriggerEnter(Collider other)
    {
        // 1. Check if we already did this
        if (hasSpawned) return;

        // 2. Check if it's a player
        if (other.CompareTag("Player"))
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        hasSpawned = true;

        // Check for missing assignments to prevent errors
        if (bossPrefab == null || spawnPoint == null)
        {
            Debug.LogError("BossTrigger is missing the Prefab or the SpawnPoint!");
            return;
        }

        // 1. Spawn the Visual Effect (if any)
        if (spawnVFX != null)
        {
            Instantiate(spawnVFX, spawnPoint.position, Quaternion.identity);
        }

        // 2. Spawn the Boss
        // The BossController.Start() will automatically trigger the UI and Music.
        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log("Boss Spawned by Trigger!");

        // 3. Optional: Destroy this trigger so it doesn't take up memory, 
        // or keep it disabled if you want to reuse it later.
        // Destroy(gameObject); 
        GetComponent<Collider>().enabled = false; // Disable collider
    }

    // This draws a line in the Editor so you can see where the boss will spawn
    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            // Draw a box where the trigger is
            Gizmos.DrawWireCube(transform.position, transform.localScale);

            // Draw a line to the spawn point
            Gizmos.DrawLine(transform.position, spawnPoint.position);

            // Draw a sphere where the boss will be
            Gizmos.DrawWireSphere(spawnPoint.position, 1f);

            // Draw a little icon
            Gizmos.DrawIcon(spawnPoint.position, "d_SkullModule Icon");
        }
    }
}
