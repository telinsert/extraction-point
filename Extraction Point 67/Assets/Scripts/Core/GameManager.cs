// In /Scripts/Core/GameManager.cs

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // The Singleton pattern
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI interactionPrompt;
    // We can store references to player stats here
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;

    // --- NEW CODE START ---
    // Public properties to check player status from other scripts
    public bool IsPlayer1Down { get; private set; }
    public bool IsPlayer2Down { get; private set; }
    public ReviveUIController reviveUIController;
    public bool IsGamePaused { get; private set; }
    // --- NEW CODE END ---
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

    private void Awake()
    {
        // --- Singleton Logic ---
        // If an instance already exists and it's not this one, destroy this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Otherwise, set the instance to this one.
        Instance = this;

        // --- Persistence ---
        // This is the magic line that keeps the GameManager alive between scenes.
        DontDestroyOnLoad(gameObject);
    }

    // --- NEW METHOD ---
    // This method will be called by a player's Health script when they die.
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

        // Check if both players are now down
        if (IsPlayer1Down && IsPlayer2Down)
        {
            TriggerGameOver();
        }
        if (reviveUIController != null)
        {
            reviveUIController.ShowPlayerDownedMessage(playerNumber);
        }
    }

    // --- NEW METHOD ---
    // We will use this later for the revive mechanic
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

    // --- NEW METHOD ---
    private void TriggerGameOver()
    {
        // For now, we'll just log a message.
        // In a future step, this will activate the Game Over UI.
        Debug.Log("GAME OVER! Both players are down.");
    }

    // This method will be called to load the next level
    public void LoadNextLevel()
    {
        // Note: Make sure your scenes are added to the Build Settings!
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there is a next scene
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("You beat the game! Or there are no more levels.");
            // Here you could load the main menu (scene 0)
            // SceneManager.LoadScene(0);
        }
    }
}