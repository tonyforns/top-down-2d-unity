using UnityEngine;

/// <summary>
/// Objeto en escena que al interactuar (E) añade ítem al inventario y se destruye (o desactiva).
/// Requiere Collider2D para que InteractionDetector lo detecte.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;
    [SerializeField] private string promptText = "Recoger";
    [Tooltip("Si es true se destruye el GameObject; si no, solo se desactiva")]
    [SerializeField] private bool destroyOnPickup = true;

    public void Interact()
    {
        if (itemData == null) return;
        var inv = Inventory.Instance;
        if (inv == null) return;

        int added = inv.AddItem(itemData, amount);
        if (added <= 0) return;

        if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    public string GetPromptText() => promptText;
}
