using UnityEngine;

public class BossSpawnTrigger : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab;

    public Transform spawnPoint;

    public GameObject spawnVFX;

    private bool hasSpawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;

        if (other.CompareTag("Player"))
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        hasSpawned = true;

        if (bossPrefab == null || spawnPoint == null)
        {
            Debug.LogError("BossTrigger is missing the Prefab or the SpawnPoint!");
            return;
        }

        if (spawnVFX != null)
        {
            Instantiate(spawnVFX, spawnPoint.position, Quaternion.identity);
        }

     
        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log("Boss Spawned by Trigger!");

        GetComponent<Collider>().enabled = false; 
    }

    
}
