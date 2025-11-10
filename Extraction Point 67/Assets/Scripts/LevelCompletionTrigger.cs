using UnityEngine;

public class LevelCompletionTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Assign the UI GameObject that appears when the level is complete.")]
    public GameObject levelCompleteUI;

    private bool levelIsCompleting = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger once and only for the player
        if (levelIsCompleting || !other.CompareTag("Player"))
        {
            return;
        }

        // Check if all enemies tagged "Enemy" are gone
        if (AreAllEnemiesDestroyed())
        {
            levelIsCompleting = true;
            Debug.Log("Level Complete! Proceeding to next level.");

            // Show the completion UI if it's assigned
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
            }

            // Tell the GameManager to load the next level after a short delay
            // Using Invoke is a simple way to create a delay.
            Invoke(nameof(GoToNextLevel), 3f); // 3-second delay
        }
        else
        {
            Debug.Log("Level not yet complete. Enemies still remain.");
            // Optionally, show a message to the player telling them to defeat all enemies.
        }
    }

    private bool AreAllEnemiesDestroyed()
    {
        // FindObjectsWithTag is okay for this check, as it only happens once at the end of a level.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }

    private void GoToNextLevel()
    {
        // The GameManager handles the scene loading logic
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("GameManager instance not found! Cannot load the next level.");
        }
    }
}
