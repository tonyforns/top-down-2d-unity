using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Colocar en el jugador (o hijo). Detecta IInteractable dentro de un trigger 2D
/// y al pulsar la tecla de interacción llama Interact() en el objetivo elegido
/// (mayor prioridad, luego más cercano).
/// Requiere un Collider2D en este GameObject con isTrigger = true.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractionDetector : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Filtro")]
    [Tooltip("Tag del jugador para ignorar su propio collider en la detección")]
    [SerializeField] private string playerTag = "Player";

    /// <summary>Objeto interactuable actualmente seleccionado (el que vería la UI).</summary>
    public IInteractable CurrentTarget { get; private set; }

    /// <summary>Se invoca cuando cambia el objetivo en rango (entra/sale el primero o cambia el elegido).</summary>
    public event System.Action<IInteractable> OnTargetChanged;

    private readonly List<IInteractable> _candidates = new List<IInteractable>();
    private Collider2D _ourCollider;

    private void Awake()
    {
        _ourCollider = GetComponent<Collider2D>();
        if (_ourCollider != null && !_ourCollider.isTrigger)
        {
            Debug.LogWarning("InteractionDetector: El Collider2D debe ser trigger. Forzando isTrigger = true.", this);
            _ourCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        RefreshCurrentTarget();
        if (Input.GetKeyDown(interactKey) && CurrentTarget != null)
        {
            CurrentTarget.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) return;
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !_candidates.Contains(interactable))
            _candidates.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) return;
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            _candidates.Remove(interactable);
    }

    private void RefreshCurrentTarget()
    {
        RemoveDestroyedCandidates();
        IInteractable newTarget = ChooseBestTarget();
        if (newTarget != CurrentTarget)
        {
            CurrentTarget = newTarget;
            OnTargetChanged?.Invoke(CurrentTarget);
        }
    }

    private void RemoveDestroyedCandidates()
    {
        for (int i = _candidates.Count - 1; i >= 0; i--)
        {
            if (_candidates[i] is MonoBehaviour mb && mb == null)
                _candidates.RemoveAt(i);
        }
    }

    private IInteractable ChooseBestTarget()
    {
        if (_candidates.Count == 0) return null;
        if (_candidates.Count == 1) return _candidates[0];

        Vector2 ourPosition = transform.position;
        IInteractable best = null;
        int bestPriority = int.MinValue;
        float bestDistSq = float.MaxValue;

        foreach (var candidate in _candidates)
        {
            if (candidate is not MonoBehaviour mb || mb == null) continue;

            int priority = candidate.GetInteractionPriority();
            float distSq = ((Vector2)mb.transform.position - ourPosition).sqrMagnitude;

            if (priority > bestPriority || (priority == bestPriority && distSq < bestDistSq))
            {
                bestPriority = priority;
                bestDistSq = distSq;
                best = candidate;
            }
        }

        return best;
    }
}
