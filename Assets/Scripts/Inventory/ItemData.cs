using UnityEngine;

/// <summary>
/// Definición de un tipo de ítem (datos compartidos). Crear desde menú: Create → Scriptable Objects → Item Data.
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identificación")]
    [Tooltip("ID único para comparar ítems del mismo tipo (ej. \"key_red\")")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    [Header("Visual")]
    [SerializeField] private Sprite icon;

    [Header("Stack")]
    [Tooltip("Si es true, varios de este ítem pueden apilarse en un solo slot")]
    [SerializeField] private bool isStackable = true;
    [Tooltip("Cantidad máxima por slot si es apilable (0 = sin límite)")]
    [SerializeField] private int maxStackSize = 99;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize <= 0 ? int.MaxValue : maxStackSize;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemId) && !string.IsNullOrEmpty(displayName))
            itemId = displayName.ToLowerInvariant().Replace(" ", "_");
    }
}
