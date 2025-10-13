// In /Scripts/Upgrades/ChestController.cs

using UnityEngine;

public class ChestController : MonoBehaviour
{
    private bool playerIsNear = false;
    private UpgradeSelectionUI upgradeUI;

    void Start()
    {
        // Find the UI in the scene. This is a simple way to connect them.
        upgradeUI = FindFirstObjectByType<UpgradeSelectionUI>();
    }

    void Update()
    {
        // Check for player interaction (e.g., pressing the "E" key)
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When a player enters the trigger, allow interaction
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            // Optionally, show a UI prompt like "Press E to open"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the player leaves, disable interaction
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            // Optionally, hide the UI prompt
        }
    }

    void OpenChest()
    {
        if (upgradeUI != null)
        {
            upgradeUI.ShowUpgradeChoices();
            // Destroy the chest so it can't be used again
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("UpgradeSelectionUI not found in the scene!");
        }
    }
}
