using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string sentence;

    [Tooltip("The text that will appear on the continue button for this line. If left empty, it will default to 'Continue'.")]
    public string buttonText = "Continue";
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Roguelike/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
}
