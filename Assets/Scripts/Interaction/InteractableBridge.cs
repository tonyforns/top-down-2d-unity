using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// Conecta un DialogueTrigger con el sistema IInteractable.
    /// Añade este componente al mismo GameObject que DialogueTrigger para que
    /// el diálogo se inicie al pulsar E (en lugar de al entrar en el trigger).
    /// Opcional: desactiva "trigger on enter" en DialogueTrigger y usa solo este bridge.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractableBridge : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueTrigger dialogueTrigger;
        [SerializeField] private string promptText = "Hablar";

        private void Reset()
        {
            dialogueTrigger = GetComponent<DialogueTrigger>();
        }

        public void Interact()
        {
            if (dialogueTrigger != null)
                dialogueTrigger.StartDialogue();
        }

        public string GetPromptText() => promptText;
    }
}
