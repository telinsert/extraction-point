using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    [Header("Dialogue")]
    public Dialogue initialDialogue;
    public Dialogue repeatedDialogue;

    private TextMeshProUGUI interactionPrompt;
    private int playersInRange = 0;


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
        if (playersInRange > 0 && Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.IsGamePaused && !GameManager.Instance.IsGameOver)
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

        
        if (!hasBeenInteractedWith)
        {
            DialogueManager.Instance.StartDialogue(initialDialogue);
            hasBeenInteractedWith = true;
        }
        else
        {
            if (repeatedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(repeatedDialogue);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(initialDialogue);
            }
        }
    }
}
