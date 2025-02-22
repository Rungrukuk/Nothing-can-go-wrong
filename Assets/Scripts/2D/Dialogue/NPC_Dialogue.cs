using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    public DialogueData dialogueData;
    [SerializeField] private GameObject cahtBoxImage;
    void Start()
    {
        cahtBoxImage.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cahtBoxImage.SetActive(true);
            PlayerDialogue.Instance.SetCurrentNPC(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cahtBoxImage.SetActive(false);
            PlayerDialogue.Instance.ClearCurrentNPC();
        }
    }
}
