
using UnityEngine;
using TMPro;

public class ChestController : MonoBehaviour
{
    private TextMeshProUGUI interactionPrompt;

    private UpgradeSelectionUI upgradeUI;
    private int playersInRange = 0;
    [Header("Audio")]
    public string openSound = "ChestOpen";

    void Start()
    {
        upgradeUI = FindFirstObjectByType<UpgradeSelectionUI>();
        interactionPrompt = GameManager.Instance.interactionPrompt; 

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

  

    void Update()
    {
        if (playersInRange > 0 && Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.IsGamePaused && !GameManager.Instance.IsGameOver)
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
        AudioManager.Instance.PlaySFXAtPosition(openSound, transform.position);

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