// In /Scripts/UI/ReviveUIController.cs

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviveUIController : MonoBehaviour
{
    [Header("UI Elements")]
    // We REMOVED the reviveUIPanel variable and REPLACED it with this:
    public GameObject contentHolder; // Drag the "ContentHolder" child object here

    public TextMeshProUGUI playerDownedText;
    public GameObject reviveProgressBackground;
    public TextMeshProUGUI reviveProgressText;

    [Header("Player References")]
    public PlayerController player1;
    public PlayerController player2;

    private Health p1_health;
    private Health p2_health;

    void Start()
    {
        if (player1 != null) p1_health = player1.GetComponent<Health>();
        if (player2 != null) p2_health = player2.GetComponent<Health>();

        // Hide the content right away, but the script itself stays active.
        HideAllUI();
    }

    // This method shows the content holder if it's hidden
    private void ShowContentHolder()
    {
        if (contentHolder != null && !contentHolder.activeSelf)
        {
            contentHolder.SetActive(true);
        }
    }

    public void ShowReviveProgress(float currentProgress, float maxTime)
    {
        ShowContentHolder(); // Make sure the parent container is visible first

        if (playerDownedText.gameObject.activeSelf)
        {
            playerDownedText.gameObject.SetActive(false);
        }

        if (reviveProgressBackground != null && !reviveProgressBackground.activeSelf)
        {
            reviveProgressBackground.SetActive(true);
        }

        if (!reviveProgressText.gameObject.activeSelf)
        {
            reviveProgressText.gameObject.SetActive(true);
        }

        int percentage = Mathf.FloorToInt((currentProgress / maxTime) * 100);
        reviveProgressText.text = $"Reviving... {percentage}%";
    }

    public void ShowPlayerDownedMessage(int downedPlayerNumber)
    {
        ShowContentHolder(); // Make sure the parent container is visible first

        if (reviveProgressBackground != null && reviveProgressBackground.activeSelf)
        {
            reviveProgressBackground.SetActive(false);
        }

        if (reviveProgressText.gameObject.activeSelf)
        {
            reviveProgressText.gameObject.SetActive(false);
        }
        if (!playerDownedText.gameObject.activeSelf)
        {
            playerDownedText.gameObject.SetActive(true);
        }
        playerDownedText.text = $"Player {downedPlayerNumber} is down! Get close to revive!";
    }

    public void HideReviveProgress()
    {
        if (reviveProgressBackground != null)
        {
            reviveProgressBackground.SetActive(false);
        }
        reviveProgressText.gameObject.SetActive(false);

        // Check if we need to revert to the downed message or hide everything
        if (p1_health != null && p1_health.GetCurrentHealth() <= 0)
        {
            ShowPlayerDownedMessage(1);
        }
        else if (p2_health != null && p2_health.GetCurrentHealth() <= 0)
        {
            ShowPlayerDownedMessage(2);
        }
        else
        {
            HideAllUI();
        }
    }

    // This method now ONLY hides the content, not the whole panel.
    public void HideAllUI()
    {
        if (contentHolder != null)
        {
            contentHolder.SetActive(false);
        }
    }
}
