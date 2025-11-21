using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI startPromptText;

    [Header("Animation Settings")]
    public float fadeSpeed = 1.5f;

    
    private bool canStart = false;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                AudioManager.Instance.PlayMusic("MenuTheme");
            }
        }
        Invoke(nameof(EnableStart), 0.5f);
    }

    void EnableStart()
    {
        canStart = true;
    }

    void Update()
    {
    
        float alpha = (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f;
        startPromptText.alpha = alpha;


        
        if (!canStart) return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        canStart = false;

        startPromptText.alpha = 1f;

        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
