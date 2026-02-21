using UnityEngine;
using Dialogue;

public class GoddessFinalDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData requiredSword;
    [SerializeField] private DialogueData noSwordDialogue;
    [SerializeField] private DialogueData finalDialogue;

    [SerializeField] private string promptText = "Hablar";

    public void Interact()
    {
        if (Inventory.Instance == null) return;

        bool hasSword = requiredSword != null && Inventory.Instance.HasItem(requiredSword, 1);
        var d = hasSword ? finalDialogue : noSwordDialogue;

        if (d != null && DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogue(d);
    }

    public string GetPromptText() => promptText;
}