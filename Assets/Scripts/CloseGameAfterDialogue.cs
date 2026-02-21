using UnityEngine;
using Dialogue;

public class CloseGameAfterDialogue : MonoBehaviour
{
    [SerializeField] private DialogueData finalDialogue;

    private bool armed;

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueComplete += OnDialogueComplete;
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueComplete -= OnDialogueComplete;
    }

    public void ArmIfFinalDialogue(DialogueData aboutToPlay)
    {
        armed = (finalDialogue != null && aboutToPlay == finalDialogue);
    }

    private void OnDialogueComplete()
    {
        if (!armed) return;

        QuitGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}