// In /Scripts/UI/GameOverUIController.cs
using System.Collections; // You might need to add this at the top of your file!
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class GameOverUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag the GameOverPanel object here.")]
    public GameObject gameOverPanel;
    [Tooltip("Drag the 'ContinuePromptText' object here.")]
    public TextMeshProUGUI continuePromptText;

    [Header("Animation Settings")]
    public float fadeSpeed = 1.5f;

    // A flag to check if the Game Over screen is active
    private bool isGameOver = false;

    // --- NEW VARIABLES ---
    [Header("Timing")]
    [Tooltip("The delay in seconds before the player can press a key to continue.")]
    public float inputDelay = 2.0f; // 2-second delay
    private bool canContinue = false;
    // --- END OF NEW VARIABLES ---

    // This method is still called by the GameManager
    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            isGameOver = true;

            // Start the coroutine to enable input after a delay.
            StartCoroutine(EnableContinueAfterDelay());
        }
    }

    // We use Update to check for input now, instead of a button.
    void Update()
    {
        // Only run this logic if the game is actually over.
        if (!isGameOver)
        {
            return;
        }

        // --- Handle Text Fading ---
        // We use Time.unscaledTime here because Time.time is frozen when timeScale is 0.
        float alpha = (Mathf.Sin(Time.unscaledTime * fadeSpeed) + 1f) / 2f;
        if (continuePromptText != null)
        {
            continuePromptText.alpha = alpha;
        }

        // --- Handle Input ---
        // --- NEW CONDITION ---
        // Only check for input if the player is allowed to continue.
        if (canContinue)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                ReturnToMainMenu();
            }
        }
    }

    // This method is now called by Update instead of the button.
    public void ReturnToMainMenu()
    {
        // Prevent this from being called multiple times
        if (!isGameOver) return;
        isGameOver = false;

        // Tell the GameManager to handle the full reset logic.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRun();
        }
        else
        {
            // Fallback in case the GameManager is missing
            Debug.LogError("GameManager instance not found! Loading scene 0 manually.");
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator EnableContinueAfterDelay()
    {
        // Wait for the specified number of seconds.
        // Importantly, this must use unscaled time to work when the game might be paused.
        yield return new WaitForSecondsRealtime(inputDelay);

        // After the wait, set the flag to true.
        canContinue = true;
        Debug.Log("Player can now continue from Game Over screen.");
    }
}