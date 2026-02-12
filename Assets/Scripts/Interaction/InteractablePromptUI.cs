using UnityEngine;
using TMPro;

/// <summary>
/// Muestra un texto de interacción en World Space encima del objeto con el que se puede interactuar.
/// Colocar en un Canvas con Render Mode = World Space. La posición del Canvas se actualiza
/// cada frame para quedar encima del objetivo actual (objeto interactuable en rango).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class InteractablePromptUI : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private GameObject panel;
    [Tooltip("Prefijo antes del texto del objeto, ej. \"E - \"")]
    [SerializeField] private string keyPrefix = "E - ";

    [Header("Posición en mundo")]
    [Tooltip("Offset desde el centro del objeto (ej. altura encima). En 2D típico: Y positivo.")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    private IInteractable _currentTarget;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (detector != null)
            detector.OnTargetChanged += OnTargetChanged;
        _currentTarget = null;
        Hide();
    }

    private void OnDisable()
    {
        if (detector != null)
            detector.OnTargetChanged -= OnTargetChanged;
    }

    private void LateUpdate()
    {
        if (_currentTarget == null) return;
        if (_currentTarget is not MonoBehaviour mb || mb == null)
        {
            Hide();
            return;
        }
        _rectTransform.position = mb.transform.position + worldOffset;
    }

    private void OnTargetChanged(IInteractable target)
    {
        _currentTarget = target;
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
        if (_rectTransform != null && target is MonoBehaviour mb2 && mb2 != null)
            _rectTransform.position = mb2.transform.position + worldOffset;
    }

    private void Hide()
    {
        _currentTarget = null;
        if (panel != null)
            panel.SetActive(false);
    }
}
