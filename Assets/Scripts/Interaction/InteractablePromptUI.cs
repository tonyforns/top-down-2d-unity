using UnityEngine;
using TMPro;

/// <summary>
/// Muestra un texto de interacción (ej. "E - Hablar") cuando el jugador tiene
/// un objeto IInteractable en rango. Asignar InteractionDetector y el TMP_Text en el Inspector.
/// </summary>
public class InteractablePromptUI : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private GameObject panel;
    [Tooltip("Prefijo antes del texto del objeto, ej. \"E - \"")]
    [SerializeField] private string keyPrefix = "E - ";

    private void OnEnable()
    {
        if (detector != null)
            detector.OnTargetChanged += OnTargetChanged;
        Hide();
    }

    private void OnDisable()
    {
        if (detector != null)
            detector.OnTargetChanged -= OnTargetChanged;
    }

    private void OnTargetChanged(IInteractable target)
    {
        if (target == null)
        {
            Hide();
            return;
        }
        string text = target.GetPromptText();
        if (promptText != null)
            promptText.text = keyPrefix + text;
        if (panel != null)
            panel.SetActive(true);
    }

    private void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
