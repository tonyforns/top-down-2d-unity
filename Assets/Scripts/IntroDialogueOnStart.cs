using UnityEngine;
using Dialogue;

public class IntroDialogueOnStart : MonoBehaviour
{
    [SerializeField] private DialogueData introDialogue;

    private void Start()
    {
        if (introDialogue == null) return;
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.StartDialogue(introDialogue);
    }
}