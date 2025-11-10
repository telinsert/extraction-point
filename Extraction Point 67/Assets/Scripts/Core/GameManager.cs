// In /Scripts/Core/GameManager.cs

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // <-- Make sure this is included!

public class GameManager : MonoBehaviour
{
    // The Singleton pattern
    public static GameManager Instance { get; private set; }

    // These references will now be found automatically at runtime.
    // You can still assign them in the Inspector for the initial scene,
    // but the code will ensure they are always up-to-date.
    public TextMeshProUGUI interactionPrompt;
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;
    public GameOverUIController gameOverUI;
    public UpgradeManager upgradeManager;
    public ReviveUIController reviveUIController;

    // Public properties to check player status from other scripts
    public bool IsPlayer1Down { get; private set; }
    public bool IsPlayer2Down { get; private set; }
    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        // --- Singleton Logic ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // --- Persistence ---
        DontDestroyOnLoad(gameObject);
    }

    // --- NEW ---
    // Subscribe to the sceneLoaded event when the object is enabled
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // --- NEW ---
    // Unsubscribe when the object is disabled to prevent memory leaks
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- NEW ---
    // This method is called every time a scene finishes loading.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("A new scene has loaded. GameManager is finding its references.");

        // Use FindFirstObjectByType to get the single instance of these critical components.
        // This will find the active objects in the newly loaded scene.
        gameOverUI = FindFirstObjectByType<GameOverUIController>();
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        reviveUIController = FindFirstObjectByType<ReviveUIController>();

        // We also need to re-link the players
        PlayerStats[] allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var p_stats in allPlayers)
        {
            if (p_stats.playerNumber == 1) player1Stats = p_stats;
            if (p_stats.playerNumber == 2) player2Stats = p_stats;
        }

        GameObject promptPanelObject = GameObject.FindGameObjectWithTag("InteractionPrompt");

        if (promptPanelObject != null)
        {
            // 2. Get the TextMeshProUGUI component from the panel's children.
            // The 'true' parameter is crucial: it tells Unity to include INACTIVE children in the search.
            interactionPrompt = promptPanelObject.GetComponentInChildren<TextMeshProUGUI>(true);

            if (interactionPrompt == null)
            {
                Debug.LogError("Found the Interaction Panel, but it has no TextMeshProUGUI component in its children!");
            }
        }
        else if (scene.buildIndex != 0) // Don't warn on the main menu
        {
            Debug.LogWarning("GameManager could not find a GameObject with the 'InteractionPrompt' tag!");
        }


        // After finding all references, reset the player down status for the new level/run.
        IsPlayer1Down = false;
        IsPlayer2Down = false;

        // Log a warning if a critical component is missing in the new scene. This helps debugging.
        if (gameOverUI == null && scene.buildIndex != 0) // Don't warn on main menu
        {
            Debug.LogWarning("GameManager could not find a GameOverUIController in the scene!");
        }
    }

    public void PauseGameForUI()
    {
        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    public void ResumeGameFromUI()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void OnPlayerDowned(int playerNumber)
    {
        if (playerNumber == 1)
        {
            IsPlayer1Down = true;
        }
        else if (playerNumber == 2)
        {
            IsPlayer2Down = true;
        }

        if (IsPlayer1Down && IsPlayer2Down)
        {
            if (reviveUIController != null)
            {
                reviveUIController.HideAllUI();
            }
            TriggerGameOver();
        }
        else
        {
            if (reviveUIController != null)
            {
                reviveUIController.ShowPlayerDownedMessage(playerNumber);
            }
        }
    }

    public void OnPlayerRevived(int playerNumber)
    {
        if (playerNumber == 1)
        {
            IsPlayer1Down = false;
        }
        else if (playerNumber == 2)
        {
            IsPlayer2Down = false;
        }
        if (reviveUIController != null)
        {
            reviveUIController.HideAllUI();
        }
    }

    private void TriggerGameOver()
    {
        if (gameOverUI != null)
        {
            // This check should now pass every time!
            gameOverUI.ShowGameOverScreen();
        }
        else
        {
            Debug.LogError("Game Over UI Controller not assigned in GameManager! It could not be found in the current scene.");
        }
    }

    public void ResetRun()
    {
        Debug.Log("Resetting run...");

        // The reset logic remains the same.
        if (player1Stats != null)
        {
            player1Stats.ResetStats();
        }
        if (player2Stats != null)
        {
            player2Stats.ResetStats();
        }

        if (upgradeManager != null)
        {
            upgradeManager.InitializeUpgradePool();
        }

        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("You beat the game! Or there are no more levels.");
            SceneManager.LoadScene(0);
        }
    }
}