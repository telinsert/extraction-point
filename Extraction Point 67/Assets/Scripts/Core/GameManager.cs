// In /Scripts/Core/GameManager.cs

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // The Singleton pattern
    public static GameManager Instance { get; private set; }

    // These references will be found automatically at runtime.
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
    public bool IsGameOver { get; private set; }


    // --- NEW ---
    // Temporary components to hold player data during scene transitions
    private PlayerStats p1_data_holder;
    private PlayerStats p2_data_holder;


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

    // Subscribe to the sceneLoaded event when the object is enabled
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unsubscribe when the object is disabled to prevent memory leaks
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called every time a scene finishes loading.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("A new scene has loaded. GameManager is finding its references.");

        // --- MODIFIED ---
        // If we are loading the main menu, don't try to find game objects.
        // Also, clean up any data holders from a previous run.
        if (scene.buildIndex == 0)
        {
            if (p1_data_holder != null) Destroy(p1_data_holder.gameObject);
            if (p2_data_holder != null) Destroy(p2_data_holder.gameObject);
            return;
        }


        // Use FindFirstObjectByType to get the single instance of these critical components.
        gameOverUI = FindFirstObjectByType<GameOverUIController>();
        upgradeManager = UpgradeManager.Instance;
        reviveUIController = FindFirstObjectByType<ReviveUIController>();


        // Re-link the players and apply any stored data from the previous level.
        PlayerStats[] allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var p_stats in allPlayers)
        {
            if (p_stats.playerNumber == 1)
            {
                player1Stats = p_stats;
                // --- NEW: If we have stored data, apply it! ---
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.player1Stats = p_stats;
                }
                if (p1_data_holder != null)
                {
                    player1Stats.CopyStatsFrom(p1_data_holder);
                    Destroy(p1_data_holder.gameObject); // Clean up the temporary object
                    Debug.Log("Player 1 data successfully transferred.");
                }
            }
            if (p_stats.playerNumber == 2)
            {
                player2Stats = p_stats;
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.player2Stats = p_stats;
                }
                // --- NEW: If we have stored data, apply it! ---
                if (p2_data_holder != null)
                {
                    player2Stats.CopyStatsFrom(p2_data_holder);
                    Destroy(p2_data_holder.gameObject); // Clean up the temporary object
                    Debug.Log("Player 2 data successfully transferred.");
                }
            }
        }
        if (player1Stats != null && player2Stats != null)
        {
            PlayerController p1Controller = player1Stats.GetComponent<PlayerController>();
            PlayerController p2Controller = player2Stats.GetComponent<PlayerController>();

            if (p1Controller != null && p2Controller != null)
            {
                p1Controller.SetTeammate(player2Stats.transform);
                p2Controller.SetTeammate(player1Stats.transform);
                Debug.Log("GameManager has successfully wired up player teammates for revive logic.");
            }
            else
            {
                Debug.LogError("Could not find PlayerController components on one or both players to wire them up!");
            }
        }

        // Find the interaction prompt UI
        GameObject promptPanelObject = GameObject.FindGameObjectWithTag("InteractionPrompt");
        if (promptPanelObject != null)
        {
            interactionPrompt = promptPanelObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if (interactionPrompt == null)
            {
                Debug.LogError("Found the Interaction Panel, but it has no TextMeshProUGUI component in its children!");
            }
        }
        else if (scene.buildIndex != 0)
        {
            Debug.LogWarning("GameManager could not find a GameObject with the 'InteractionPrompt' tag!");
        }

        // After finding all references, reset the player down status for the new level.
        IsPlayer1Down = false;
        IsPlayer2Down = false;

        // Log a warning if a critical component is missing in the new scene.
        if (gameOverUI == null && scene.buildIndex != 0)
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
        if (playerNumber == 1) IsPlayer1Down = true;
        else if (playerNumber == 2) IsPlayer2Down = true;

        if (IsPlayer1Down && IsPlayer2Down)
        {
            if (reviveUIController != null) reviveUIController.HideAllUI();
            TriggerGameOver();
        }
        else
        {
            if (reviveUIController != null) reviveUIController.ShowPlayerDownedMessage(playerNumber);
        }
    }

    public void OnPlayerRevived(int playerNumber)
    {
        if (playerNumber == 1) IsPlayer1Down = false;
        else if (playerNumber == 2) IsPlayer2Down = false;
        
        if (reviveUIController != null) reviveUIController.HideAllUI();
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOverScreen();
        }
        else
        {
            Debug.LogError("Game Over UI Controller not assigned or found in the current scene!");
        }
    }

    // --- MODIFIED ---
    public void ResetRun()
    {
        Debug.Log("Resetting run...");
        ResumeGameFromUI();
        IsGameOver = false;

        // --- NEW ---
        // Clean up any lingering data holders to ensure a fresh start.
        if (p1_data_holder != null) Destroy(p1_data_holder.gameObject);
        if (p2_data_holder != null) Destroy(p2_data_holder.gameObject);
        
        // The reset logic remains the same.
        if (player1Stats != null) player1Stats.ResetStats();
        if (player2Stats != null) player2Stats.ResetStats();
        if (upgradeManager != null) upgradeManager.InitializeUpgradePool();
        
        SceneManager.LoadScene(0);
    }

    // --- MODIFIED ---
    public void LoadNextLevel()
    {
        // --- NEW: Store player data before leaving the current scene ---
        StorePlayerData();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("You beat the game! Returning to Main Menu.");
            SceneManager.LoadScene(0);
        }
    }
    
    // --- NEW METHOD ---
    private void StorePlayerData()
    {
        // Create temporary GameObjects with PlayerStats components to hold the data.
        // These objects are marked with DontDestroyOnLoad so they survive the scene change.
        if (player1Stats != null)
        {
            GameObject p1_temp = new GameObject("P1_DataHolder");
            p1_data_holder = p1_temp.AddComponent<PlayerStats>();
            p1_data_holder.CopyStatsFrom(player1Stats); // Assumes PlayerStats has a CopyStatsFrom method
            DontDestroyOnLoad(p1_temp);
            Debug.Log("Player 1 data stored for transition.");
        }
        if (player2Stats != null)
        {
            GameObject p2_temp = new GameObject("P2_DataHolder");
            p2_data_holder = p2_temp.AddComponent<PlayerStats>();
            p2_data_holder.CopyStatsFrom(player2Stats);
            DontDestroyOnLoad(p2_temp);
            Debug.Log("Player 2 data stored for transition.");
        }
    }
}