// In /Scripts/Upgrades/ChestController.cs

using UnityEngine;
using TMPro;

public class ChestController : MonoBehaviour
{
    // We get this from the GameManager now, so no public variable is needed here.
    private TextMeshProUGUI interactionPrompt;

    private UpgradeSelectionUI upgradeUI;
    private int playersInRange = 0;

    void Start()
    {
        // Get references from our reliable sources
        upgradeUI = FindFirstObjectByType<UpgradeSelectionUI>();
        interactionPrompt = GameManager.Instance.interactionPrompt; // <-- Get the prompt from the manager

        // Ensure the prompt is hidden when the chest first spawns
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    // The rest of the script (Update, OnTriggerEnter, OnTriggerExit, OpenChest)
    // is now guaranteed to work and needs NO CHANGES.

    void Update()
    {
        if (playersInRange > 0 && Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.IsGamePaused)
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange++;
            if (playersInRange == 1 && interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange--;
            if (playersInRange == 0 && interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }
    }

    void OpenChest()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        if (upgradeUI != null)
        {
            upgradeUI.ShowUpgradeChoices();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("UpgradeSelectionUI not found in the scene!");
        }
    }
}