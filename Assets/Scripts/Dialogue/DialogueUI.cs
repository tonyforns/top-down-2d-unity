using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    /// <summary>
    /// Componente de UI que muestra una línea de diálogo con TextMesh Pro,
    /// animación de escritura y imagen del personaje.
    /// Asignar referencias en el Inspector: DialogueText (TMP), PortraitImage, opcional NameText.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private GameObject dialoguePanel;

        [Header("Configuración tipo de escritura")]
        [SerializeField] [Min(1f)] private float defaultTypewriterSpeed = 30f;

        /// <summary>Se invoca cuando termina de mostrarse la línea actual (incluida la animación).</summary>
        public event Action OnLineDisplayComplete;

        private Coroutine _typewriterCoroutine;
        private bool _isTyping;
        private string _currentFullText;

        private void Awake()
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }

        /// <summary>Muestra una línea de diálogo con animación de escritura y retrato.</summary>
        public void ShowLine(DialogueLine line)
        {
            if (line == null)
            {
                Debug.LogWarning("DialogueUI: Se intentó mostrar una línea nula.");
                return;
            }

            StopTypewriter();

            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);

            if (portraitImage != null)
            {
                portraitImage.enabled = line.CharacterPortrait != null;
                portraitImage.sprite = line.CharacterPortrait;
            }

            if (characterNameText != null)
            {
                characterNameText.text = line.CharacterName ?? string.Empty;
                characterNameText.gameObject.SetActive(!string.IsNullOrEmpty(line.CharacterName));
            }

            float speed = line.TypewriterSpeed > 0f ? line.TypewriterSpeed : defaultTypewriterSpeed;
            _typewriterCoroutine = StartCoroutine(TypewriterRoutine(line.Text, speed));
        }

        /// <summary>Si está escribiendo, muestra todo el texto de golpe y termina la línea. Si no, no hace nada.</summary>
        public void SkipTypewriter()
        {
            if (!_isTyping) return;
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            if (dialogueText != null && !string.IsNullOrEmpty(_currentFullText))
                dialogueText.text = _currentFullText;
            _isTyping = false;
            OnLineDisplayComplete?.Invoke();
        }

        /// <summary>Oculta el panel de diálogo.</summary>
        public void Hide()
        {
            StopTypewriter();
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }

        /// <summary>Indica si la animación de escritura está en curso.</summary>
        public bool IsTyping => _isTyping;

        private void StopTypewriter()
        {
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            _isTyping = false;
        }

        private IEnumerator TypewriterRoutine(string fullText, float charactersPerSecond)
        {
            _currentFullText = fullText ?? string.Empty;
            _isTyping = true;
            if (dialogueText != null)
                dialogueText.text = string.Empty;

            if (string.IsNullOrEmpty(fullText))
            {
                _isTyping = false;
                OnLineDisplayComplete?.Invoke();
                yield break;
            }

            float delay = 1f / Mathf.Max(charactersPerSecond, 1f);
            int length = fullText.Length;
            int current = 0;

            while (current < length)
            {
                current++;
                if (dialogueText != null)
                    dialogueText.text = fullText.Substring(0, current);
                yield return new WaitForSeconds(delay);
            }

            _isTyping = false;
            _typewriterCoroutine = null;
            OnLineDisplayComplete?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (defaultTypewriterSpeed < 1f) defaultTypewriterSpeed = 1f;
        }
#endif
    }
}
