using UnityEngine;

public class LevelCompletionTrigger : MonoBehaviour
{
    public GameObject levelCompleteUI; // UI element to display level completion message

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Check if all enemies are destroyed
            if (AreAllEnemiesDestroyed())
            {
                // Display level completion message
                if (levelCompleteUI != null)
                {
                    levelCompleteUI.SetActive(true);
                }

                Debug.Log("Level Completed!");
            }
            else
            {
                Debug.Log("Not all enemies are destroyed yet!");
            }
        }
    }

    private bool AreAllEnemiesDestroyed()
    {
        // Find all GameObjects with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Check if any enemies still exist
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) // If any enemy still exists
            {
                return false;
            }
        }
        return true; // All enemies are destroyed
    }
}
