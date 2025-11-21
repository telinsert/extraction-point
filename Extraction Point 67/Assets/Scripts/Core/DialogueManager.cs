using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialogueContentHolder; 
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;
    public TextMeshProUGUI continueButtonText;

    private Queue<DialogueLine> sentences;

    private void Awake()
    {
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
        continueButton.onClick.AddListener(DisplayNextSentence);

        if (dialogueContentHolder != null)
        {
            dialogueContentHolder.SetActive(false);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        GameManager.Instance.PauseGameForUI(); 
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

        
        if (!string.IsNullOrEmpty(currentLine.buttonText))
        {
            continueButtonText.text = currentLine.buttonText;
        }
        else
        {
            continueButtonText.text = "Continue";
        }
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    void Update()
    {
      
        if (dialogueContentHolder.activeInHierarchy)
        {
            
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                DisplayNextSentence();
            }
        }
    }

    void EndDialogue()
    {
        dialogueContentHolder.SetActive(false);
        
        GameManager.Instance.ResumeGameFromUI();
    }
}