using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "Dialogue/NPC Dialogue")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueLine
    {
        [TagSelector] public string speakerTag;
        [TextArea(2, 5)] public string text;
    }

    public Sprite portrait;
    public DialogueLine[] dialogueLines;
}
