// In /Scripts/Core/GameManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // The Singleton pattern
    public static GameManager Instance { get; private set; }

    // We can store references to player stats here
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;

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
