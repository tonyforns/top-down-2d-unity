using UnityEngine;

/// <summary>
/// Refresca la UI de slots cuando cambia el inventario. Asignar Inventory y la lista de InventorySlotUI en el Inspector.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventorySlotUI[] slotUIs;

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        if (inventory == null || slotUIs == null) return;
        int count = Mathf.Min(inventory.SlotCount, slotUIs.Length);
        for (int i = 0; i < count; i++)
            slotUIs[i].Set(inventory.GetSlot(i));
        for (int i = count; i < slotUIs.Length; i++)
            slotUIs[i].Set(default);
    }
}
