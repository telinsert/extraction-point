// In /Scripts/NPC/NPCController.cs
using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    // --- MODIFIED ---
    [Header("Dialogue")]
    [Tooltip("The dialogue to play the very first time the player interacts.")]
    public Dialogue initialDialogue;
    [Tooltip("The dialogue to play on all subsequent interactions. If left empty, the initial dialogue will be repeated.")]
    public Dialogue repeatedDialogue;

    private TextMeshProUGUI interactionPrompt;
    private int playersInRange = 0;

    // --- NEW ---
    // This flag will track if the NPC has been talked to already.
    private bool hasBeenInteractedWith = false;

    void Start()
    {
        interactionPrompt = GameManager.Instance.interactionPrompt;
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playersInRange > 0 && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
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

    public void TriggerDialogue()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        // --- MODIFIED LOGIC ---
        // Check if we have interacted before.
        if (!hasBeenInteractedWith)
        {
            // If this is the first time, use the initial dialogue.
            DialogueManager.Instance.StartDialogue(initialDialogue);
            // Then, set the flag so we know the first interaction is done.
            hasBeenInteractedWith = true;
        }
        else
        {
            // If we have talked before, check if a 'repeatedDialogue' asset has been assigned.
            if (repeatedDialogue != null)
            {
                // If it has, use it.
                DialogueManager.Instance.StartDialogue(repeatedDialogue);
            }
            else
            {
                // If not, just fall back to the initial dialogue again.
                DialogueManager.Instance.StartDialogue(initialDialogue);
            }
        }
    }
}
