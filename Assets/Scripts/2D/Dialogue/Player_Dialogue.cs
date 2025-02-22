using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogue : MonoBehaviour
{
    public static PlayerDialogue Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image speakerImage;
    [SerializeField] private Text dialogueText;

    [SerializeField] private Sprite playerPortrait;

    private NPC_Dialogue currentNPC;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (currentNPC != null && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive)
                StartDialogue();
            else
                ContinueDialogue();
        }
    }

    public void SetCurrentNPC(NPC_Dialogue npc)
    {
        currentNPC = npc;
    }

    public void ClearCurrentNPC()
    {
        currentNPC = null;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        currentLineIndex = 0;
    }

    private void StartDialogue()
    {
        if (currentNPC?.dialogueData != null && currentNPC.dialogueData.dialogueLines.Length > 0)
        {
            isDialogueActive = true;
            currentLineIndex = 0;
            DisplayDialogue();
        }
    }

    private void ContinueDialogue()
    {
        currentLineIndex++;
        if (currentLineIndex < currentNPC.dialogueData.dialogueLines.Length)
        {
            DisplayDialogue();
        }
        else
        {
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
        }
    }

    private void DisplayDialogue()
    {
        var dialogueLine = currentNPC.dialogueData.dialogueLines[currentLineIndex];

        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLine.text;

        if (dialogueLine.speakerTag == gameObject.tag)
        {
            speakerImage.sprite = playerPortrait;
        }
        else
        {
            speakerImage.sprite = currentNPC.dialogueData.portrait;
        }
    }
}
