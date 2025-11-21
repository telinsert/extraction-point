using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string sentence;

    public string buttonText = "Continue";
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Roguelike/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
}
