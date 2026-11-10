using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Line Data", fileName = "Line - ")]
public class DialogueLineSO : ScriptableObject
{
    [Header("Dialogue info")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speaker;

    [Header("Text options")]
    [TextArea] public string[] textLine; // NPC's

    [Header("Choices info")]
    [TextArea] public string playerChoiceAnswer; // player's
    public DialogueLineSO[] choiceLines;
    
    [Header("Dialogue action")]
    public DialogueActionType actionType;
    [TextArea] public string actionLine;

    public string GetRandomLine()
    {
        return textLine[Random.Range(0, textLine.Length)];
    }

    internal string GetFirstLine() => textLine[0];
}
