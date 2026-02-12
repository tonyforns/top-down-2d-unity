using System;
using UnityEngine;

/// <summary>
/// Comprueba si se puede craftar una receta y ejecuta el crafteo (consumir ingredientes, añadir resultado).
/// Usa el inventario singleton. Opcionalmente puede ser singleton para tener una lista de recetas disponibles.
/// </summary>
public class CraftingSystem : MonoBehaviour
{
    [Header("Recetas disponibles")]
    [Tooltip("Recetas que este sistema puede craftar (ej. las de una mesa de crafteo)")]
    [SerializeField] private CraftingRecipe[] recipes;

    /// <summary>Se invoca cuando se crafta correctamente (receta, cantidad resultante).</summary>
    public event Action<CraftingRecipe, int> OnCrafted;

    /// <summary>Recetas configuradas en el Inspector.</summary>
    public CraftingRecipe[] Recipes => recipes;

    /// <summary>True si el jugador tiene todos los ingredientes y espacio para el resultado.</summary>
    public bool CanCraft(CraftingRecipe recipe)
    {
        if (recipe == null || !recipe.IsValid) return false;
        var inv = Inventory.Instance;
        if (inv == null) return false;

        foreach (var ing in recipe.Ingredients)
        {
            if (!ing.IsValid) return false;
            if (!inv.HasItem(ing.Item, ing.Amount)) return false;
        }

        return inv.CountSpaceFor(recipe.Result.Item, recipe.Result.Amount) >= recipe.Result.Amount;
    }

    /// <summary>Consume ingredientes y añade el resultado. Devuelve true si se craftó.</summary>
    public bool Craft(CraftingRecipe recipe)
    {
        if (recipe == null || !recipe.IsValid) return false;
        var inv = Inventory.Instance;
        if (inv == null) return false;

        if (!CanCraft(recipe)) return false;

        foreach (var ing in recipe.Ingredients)
            inv.RemoveItem(ing.Item, ing.Amount);

        int added = inv.AddItem(recipe.Result.Item, recipe.Result.Amount);
        if (added > 0)
            OnCrafted?.Invoke(recipe, added);
        return added > 0;
    }
}
