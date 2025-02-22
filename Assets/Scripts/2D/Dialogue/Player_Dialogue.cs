using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerDialogue : MonoBehaviour
{
    public static PlayerDialogue Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image speakerImage;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float typingSoundSpeed = 3;
    [SerializeField] private AudioSource playerVoice;
    [SerializeField] private AudioSource NPC_Voice;

    public bool isDialogueActive = false;

    private NPC_Dialogue currentNPC;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        dialoguePanel.SetActive(false);
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
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentNPC.dialogueData.dialogueLines[currentLineIndex].text;
            typingCoroutine = null;
            return;
        }

        currentLineIndex++;
        if (currentLineIndex < currentNPC.dialogueData.dialogueLines.Length)
        {
            DisplayDialogue();
        }
        else
        {
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
            if (currentNPC.name == "Dr. Mustache")
            {
                SceneManager.LoadScene("Scene 3D");
            }
        }
    }

    private void DisplayDialogue()
    {
        var dialogueLine = currentNPC.dialogueData.dialogueLines[currentLineIndex];

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        if (dialogueLine.speakerTag == gameObject.tag)
        {
            speakerImage.sprite = playerPortrait;
            playerVoice.Play();
        }
        else
        {
            speakerImage.sprite = currentNPC.dialogueData.portrait;
            NPC_Voice.Play();
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogueLine.text));
    }


    private IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        int charCount = 0;
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            charCount++;

            if (charCount >= typingSoundSpeed)
            {
                if (speakerImage.sprite == playerPortrait)
                {
                    playerVoice.Play();
                }
                else
                {
                    NPC_Voice.Play();
                }
                charCount = 0;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

}
