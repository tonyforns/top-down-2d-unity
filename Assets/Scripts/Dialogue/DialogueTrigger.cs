using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// Inicia un diálogo al entrar en un trigger 2D o al llamar StartDialogue() (desde UI, evento, etc.).
    /// Opcional: marcar "Use Once" para que solo se reproduzca una vez.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private DialogueData dialogueData;
        [Tooltip("Si está activo, el diálogo solo se dispara una vez")]
        [SerializeField] private bool useOnce = true;

        private bool _alreadyTriggered;

        private void Reset()
        {
            dialogueManager = DialogueManager.Instance;
        }

        /// <summary>Inicia el diálogo asignado (útil para llamar desde botones o eventos).</summary>
        public void StartDialogue()
        {
            if (dialogueManager == null) dialogueManager = DialogueManager.Instance;
            if (dialogueManager == null || dialogueData == null) return;
            if (useOnce && _alreadyTriggered) return;

            _alreadyTriggered = true;
            dialogueManager.StartDialogue(dialogueData);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (useOnce && _alreadyTriggered) return;
            if (dialogueManager == null) dialogueManager = DialogueManager.Instance;
            if (dialogueManager == null || dialogueData == null) return;
            if (!other.CompareTag("Player")) return;

            _alreadyTriggered = true;
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}
