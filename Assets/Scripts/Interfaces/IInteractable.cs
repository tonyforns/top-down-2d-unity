/// <summary>
/// Contrato para cualquier objeto con el que el jugador pueda interactuar (E, botón, etc.).
/// Implementar en el MonoBehaviour del objeto; el InteractionDetector del jugador llamará Interact().
/// </summary>
public interface IInteractable
{
    /// <summary>Ejecuta la acción de interacción (abrir, hablar, activar palanca, etc.).</summary>
    void Interact();

    /// <summary>Texto mostrado en la UI de interacción (ej. "Hablar", "Abrir cofre"). Por defecto "Interactuar".</summary>
    string GetPromptText() => "Interactuar";

    /// <summary>Prioridad al haber varios en rango; mayor valor = mayor prioridad. Por defecto 0.</summary>
    int GetInteractionPriority() => 0;
}
