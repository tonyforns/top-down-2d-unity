using System;
using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// Orquesta el flujo de diálogos: inicia diálogos, avanza líneas con input y notifica cuando termina.
    /// Asignar DialogueUI en el Inspector. Llamar StartDialogue(dialogueData) para comenzar.
    /// </summary>
    public class DialogueManager : Singleton<DialogueManager>
    {
        [SerializeField] private DialogueUI dialogueUI;

        [Header("Input")]
        [Tooltip("Tecla para avanzar/saltar (también se usa clic del ratón)")]
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;

        /// <summary>Se invoca cuando termina todo el diálogo (se mostraron todas las líneas).</summary>
        public event Action OnDialogueComplete;

        private DialogueData _currentDialogue;
        private int _currentLineIndex;
        private bool _isRunning;

        private void OnEnable()
        {
            if (dialogueUI != null)
                dialogueUI.OnLineDisplayComplete += OnLineDisplayComplete;
        }

        private void OnDisable()
        {
            if (dialogueUI != null)
                dialogueUI.OnLineDisplayComplete -= OnLineDisplayComplete;
        }

        private void Update()
        {
            if (!_isRunning) return;

            if (dialogueUI.IsTyping)
            {
                if (WasAdvancePressed())
                    dialogueUI.SkipTypewriter();
            }
            else
            {
                if (WasAdvancePressed())
                    AdvanceToNextLine();
            }
        }

        /// <summary>Inicia la secuencia de diálogo. Si ya hay uno en curso, se reemplaza.</summary>
        public void StartDialogue(DialogueData data)
        {
            if (data == null || data.Lines == null || data.Lines.Length == 0)
            {
                Debug.LogWarning("DialogueManager: DialogueData nulo o sin líneas.");
                OnDialogueComplete?.Invoke();
                return;
            }

            if (dialogueUI == null)
            {
                Debug.LogError("DialogueManager: No hay DialogueUI asignado.");
                return;
            }

            _currentDialogue = data;
            _currentLineIndex = 0;
            _isRunning = true;
            ShowCurrentLine();
        }

        /// <summary>Detiene el diálogo y oculta la UI.</summary>
        public void StopDialogue()
        {
            _isRunning = false;
            _currentDialogue = null;
            dialogueUI?.Hide();
        }

        /// <summary>Indica si hay un diálogo activo.</summary>
        public bool IsDialogueActive => _isRunning;

        private void OnLineDisplayComplete()
        {
            // El usuario avanza con input en Update; aquí solo podríamos auto-avanzar si se desea.
        }

        private void AdvanceToNextLine()
        {
            if (_currentDialogue == null || _currentLineIndex >= _currentDialogue.Lines.Length - 1)
            {
                EndDialogue();
                return;
            }

            _currentLineIndex++;
            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            if (_currentDialogue == null || _currentLineIndex >= _currentDialogue.Lines.Length)
            {
                EndDialogue();
                return;
            }

            dialogueUI.ShowLine(_currentDialogue.Lines[_currentLineIndex]);
        }

        private void EndDialogue()
        {
            _isRunning = false;
            dialogueUI?.Hide();
            OnDialogueComplete?.Invoke();
        }

        private bool WasAdvancePressed()
        {
            return Input.GetKeyDown(advanceKey) || Input.GetMouseButtonDown(0);
        }
    }
}
