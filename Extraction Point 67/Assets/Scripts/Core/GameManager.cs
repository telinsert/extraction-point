
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI interactionPrompt;
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;
    public GameOverUIController gameOverUI;
    public UpgradeManager upgradeManager;
    public ReviveUIController reviveUIController;

    public bool IsPlayer1Down { get; private set; }
    public bool IsPlayer2Down { get; private set; }
    public bool IsGamePaused { get; private set; }
    public bool IsGameOver { get; private set; }


    private PlayerStateData p1_data_holder;
    private PlayerStateData p2_data_holder;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("A new scene has loaded. GameManager is finding its references.");
        

        if (scene.buildIndex == 0)
        {
            return;
        }
        if (AudioManager.Instance != null)
        {
            if (scene.buildIndex == 0 || scene.buildIndex == 4) // Main Menu
            {
                AudioManager.Instance.PlayMusic("MenuTheme");
            }
            else if (scene.buildIndex == 1) 
            {
                AudioManager.Instance.PlayMusic("ForestTheme");
            }
            else if (scene.buildIndex == 2) 
            {
                AudioManager.Instance.PlayMusic("BeachTheme");
            }
            else if (scene.buildIndex == 3) 
            {
                AudioManager.Instance.PlayMusic("CityTheme");
            }
        }


        gameOverUI = FindFirstObjectByType<GameOverUIController>();
        upgradeManager = UpgradeManager.Instance;
        reviveUIController = FindFirstObjectByType<ReviveUIController>();


        PlayerStats[] allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var p_stats in allPlayers)
        {
            if (p_stats.playerNumber == 1)
            {
                player1Stats = p_stats;
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.player1Stats = p_stats;
                }
                if (p1_data_holder != null)
                {
                    player1Stats.ApplyStateData(p1_data_holder);
                    p1_data_holder = null;
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
                if (p2_data_holder != null)
                {
                    player2Stats.ApplyStateData(p2_data_holder);
                    p2_data_holder = null;
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
            }
            else
            {
                Debug.LogError("Could not find PlayerController components on one or both players to wire them up!");
            }
        }

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

        IsPlayer1Down = false;
        IsPlayer2Down = false;

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
        AudioManager.Instance.PlaySFX("GameOver");


        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOverScreen();
        }
        else
        {
            Debug.LogError("Game Over UI Controller not assigned or found in the current scene!");
        }
    }

    public void ResetRun()
    {
        Debug.Log("Resetting run...");
        ResumeGameFromUI();
        IsGameOver = false;

        p1_data_holder = null;
        p2_data_holder = null;

        if (player1Stats != null) player1Stats.ResetStats();
        if (player2Stats != null) player2Stats.ResetStats();
        if (upgradeManager != null) upgradeManager.InitializeUpgradePool();
        
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        StorePlayerData();
        AudioManager.Instance.PlaySFX("LevelComplete");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int cityLevelBuildIndex = 3;

        if (currentSceneIndex == cityLevelBuildIndex)
        {
            SceneManager.LoadScene("DemoEndScreen"); 
        }
        else
        {
            int nextSceneIndex = currentSceneIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("You beat the game! Returning to Main Menu.");
                SceneManager.LoadScene("DemoEndScreen");
            }
        }
    }
    
    private void StorePlayerData()
    {
        if (player1Stats != null)
        {
            p1_data_holder = new PlayerStateData(player1Stats);
            Debug.Log("Player 1 data stored for transition.");
        }
        if (player2Stats != null)
        {
            p2_data_holder = new PlayerStateData(player2Stats);
            Debug.Log("Player 2 data stored for transition.");
        }
    }
}