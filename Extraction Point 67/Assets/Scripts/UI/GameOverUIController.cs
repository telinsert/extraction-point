using System.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class GameOverUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI continuePromptText;

    [Header("Animation Settings")]
    public float fadeSpeed = 1.5f;

    private bool isGameOver = false;

    [Header("Timing")]
    public float inputDelay = 2.0f; 
    private bool canContinue = false;

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            isGameOver = true;

            StartCoroutine(EnableContinueAfterDelay());
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            return;
        }

        float alpha = (Mathf.Sin(Time.unscaledTime * fadeSpeed) + 1f) / 2f;
        if (continuePromptText != null)
        {
            continuePromptText.alpha = alpha;
        }

        if (canContinue)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                ReturnToMainMenu();
            }
        }
    }

    public void ReturnToMainMenu()
    {
        if (!isGameOver) return;
        isGameOver = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRun();
        }
        else
        {
            Debug.LogError("GameManager instance not found! Loading scene 0 manually.");
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator EnableContinueAfterDelay()
    {
        yield return new WaitForSecondsRealtime(inputDelay);

        canContinue = true;
        Debug.Log("Player can now continue from Game Over screen.");
    }
}