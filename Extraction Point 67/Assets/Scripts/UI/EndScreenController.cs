using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour
{
    private bool canContinue = false;

    void Start()
    {
        // Enable input after a short delay to prevent accidental skips.
        Invoke(nameof(EnableContinue), 1.5f);
    }

    void EnableContinue()
    {
        canContinue = true;
    }

    void Update()
    {
        // If the player is allowed to continue and presses any key...
        if (canContinue && Input.anyKeyDown)
        {
            // Prevent this from running multiple times.
            canContinue = false;

            // Load the Main Menu scene (which is at build index 0).
            GameManager.Instance.ResetRun();
        }
    }
}
