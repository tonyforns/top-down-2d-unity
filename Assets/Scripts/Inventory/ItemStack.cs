using System;

/// <summary>
/// Representa un slot de inventario en runtime: tipo de ítem + cantidad.
/// </summary>
[Serializable]
public struct ItemStack
{
    public ItemData Data;
    public int Amount;

    public bool IsEmpty => Data == null || Amount <= 0;

    public ItemStack(ItemData data, int amount = 1)
    {
        Data = data;
        Amount = Math.Max(0, amount);
    }

    /// <summary>Cantidad que se puede añadir a este stack sin superar el máximo.</summary>
    public int SpaceLeft => Data == null ? 0 : Math.Max(0, Data.MaxStackSize - Amount);

    public bool CanStackWith(ItemStack other) =>
        !IsEmpty && !other.IsEmpty && Data == other.Data && SpaceLeft > 0;
}
