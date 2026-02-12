using UnityEngine;

/// <summary>
/// Par ítem + cantidad para usar en recetas (ingredientes y resultados).
/// </summary>
[System.Serializable]
public struct RecipeIngredient
{
    public ItemData Item;
    public int Amount;

    public RecipeIngredient(ItemData item, int amount = 1)
    {
        Item = item;
        Amount = amount;
    }

    public bool IsValid => Item != null && Amount > 0;
}
