using UnityEngine;

public class LevelCompletionTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelCompleteUI;

    private bool levelIsCompleting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (levelIsCompleting || !other.CompareTag("Player"))
        {
            return;
        }

        if (AreAllEnemiesDestroyed())
        {
            levelIsCompleting = true;
            Debug.Log("Level Complete! Proceeding to next level.");

            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
            }

            Invoke(nameof(GoToNextLevel), 3f); 
        }
        else
        {
            Debug.Log("Level not yet complete. Enemies still remain.");
        }
    }

    private bool AreAllEnemiesDestroyed()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }

    private void GoToNextLevel()
    {
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
