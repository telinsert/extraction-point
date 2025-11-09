// In /Scripts/UI/MainMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // We need this to control TextMeshPro elements

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag the 'StartPromptText' object here.")]
    public TextMeshProUGUI startPromptText;

    [Header("Animation Settings")]
    [Tooltip("How fast the text fades in and out.")]
    public float fadeSpeed = 1.5f;

    // We only want to check for input after a brief delay
    // to prevent accidentally starting the game while loading.
    private bool canStart = false;

    void Start()
    {
        // A small delay before the player can start the game.
        Invoke(nameof(EnableStart), 0.5f);
    }

    // This method is called by Invoke in Start()
    void EnableStart()
    {
        canStart = true;
    }

    void Update()
    {
        // --- 1. Handle the Text Fading Animation ---

        // We use a Sine wave to create a smooth, repeating pulse effect.
        // The result of Sin() is -1 to 1. We remap it to 0 to 1 for the alpha.
        float alpha = (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f;
        startPromptText.alpha = alpha;


        // --- 2. Handle the Input to Start the Game ---

        // If we can't start yet, do nothing.
        if (!canStart) return;

        // Check if any keyboard key was pressed OR any mouse button was clicked.
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        // Prevent the game from being started multiple times.
        canStart = false;

        // Optional: Make the text solid white to show input was received.
        startPromptText.alpha = 1f;

        // Load the first level (build index 1).
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
