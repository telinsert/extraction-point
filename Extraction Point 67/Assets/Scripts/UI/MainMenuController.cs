// In /Scripts/UI/MainMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MainMenuController : MonoBehaviour
{
    // This public method will be called by our Start Game button.
    public void StartGame()
    {
        // SceneManager.LoadScene() loads a scene by its name or build index.
        // We want to load the next scene in our build order, which is index 1.
        SceneManager.LoadScene(1);
    }

    // This public method will be for a Quit button if you add one.
    public void QuitGame()
    {
        // This line quits the application.
        // It only works in a built game, not in the Unity Editor.
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
