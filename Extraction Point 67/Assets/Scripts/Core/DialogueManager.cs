// In /Scripts/Core/DialogueManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    // --- THIS IS THE RENAMED VARIABLE ---
    public GameObject dialogueContentHolder; // Drag the child "ContentHolder" object here
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;
    public TextMeshProUGUI continueButtonText;

    private Queue<DialogueLine> sentences;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        sentences = new Queue<DialogueLine>();
    }

    private void Start()
    {
        // Add a listener to the button to call the DisplayNextSentence method
        continueButton.onClick.AddListener(DisplayNextSentence);

        // Ensure the content is hidden at the start of the game
        if (dialogueContentHolder != null)
        {
            dialogueContentHolder.SetActive(false);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        GameManager.Instance.PauseGameForUI(); // Use the central method
        dialogueContentHolder.SetActive(true);

        sentences.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = sentences.Dequeue();
        speakerNameText.text = currentLine.speakerName;
        dialogueText.text = currentLine.sentence;

        // --- MODIFIED LOGIC ---
        // We no longer need the special check for the last sentence.
        // The button text is now controlled directly by the data.
        if (!string.IsNullOrEmpty(currentLine.buttonText))
        {
            continueButtonText.text = currentLine.buttonText;
        }
        else
        {
            // A fallback in case the text was left empty in the inspector.
            continueButtonText.text = "Continue";
        }
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    void Update()
    {
        // First, check if the dialogue UI is even active.
        // The 'activeInHierarchy' property is a reliable check.
        if (dialogueContentHolder.activeInHierarchy)
        {
            // Check if the player pressed the "Enter" key on the main keyboard OR the number pad.
            // We use GetKeyDown so it only fires once per press, not every frame the key is held down.
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // If a key was pressed, simply call the same method the button does.
                DisplayNextSentence();
            }
        }
    }

    void EndDialogue()
    {
        dialogueContentHolder.SetActive(false);
        // --- MODIFIED ---
        GameManager.Instance.ResumeGameFromUI();
    }
}