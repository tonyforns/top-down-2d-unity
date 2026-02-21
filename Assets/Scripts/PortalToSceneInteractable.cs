using UnityEngine;
using UnityEngine.SceneManagement;
using Dialogue;

[RequireComponent(typeof(Collider2D))]
public class PortalToSceneInteractable : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    [SerializeField] private string sceneName;

    [Header("Lock")]
    [SerializeField] private bool unlocked = false;
    [SerializeField] private DialogueData lockedDialogue;

    [Header("UI Prompt")]
    [SerializeField] private string promptText = "Teletransportarse";

    public void Interact()
    {
        if (!unlocked)
        {
            if (lockedDialogue != null && DialogueManager.Instance != null)
                DialogueManager.Instance.StartDialogue(lockedDialogue);

            return;
        }

        if (!string.IsNullOrWhiteSpace(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    public string GetPromptText() => promptText;

    public void SetUnlocked(bool value)
    {
        unlocked = value;
    }
}