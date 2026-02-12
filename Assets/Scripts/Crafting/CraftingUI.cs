using UnityEngine;

/// <summary>
/// Muestra la lista de recetas del CraftingSystem y refresca el estado de cada una
/// cuando cambia el inventario (para habilitar/deshabilitar Craft).
/// </summary>
public class CraftingUI : MonoBehaviour
{
    [SerializeField] private CraftingSystem craftingSystem;
    [SerializeField] private CraftingRecipeEntryUI[] recipeEntries;
    [Tooltip("Si está asignado, se suscribe a su OnInventoryChanged para refrescar")]
    [SerializeField] private Inventory inventory;

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += RefreshAllEntries;
        RefreshAllEntries();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshAllEntries;
    }

    private void Start()
    {
        if (craftingSystem == null) return;
        if (inventory == null)
            inventory = Inventory.Instance;

        var recipes = craftingSystem.Recipes;
        if (recipes == null || recipeEntries == null) return;

        int count = Mathf.Min(recipes.Length, recipeEntries.Length);
        for (int i = 0; i < count; i++)
            recipeEntries[i].Set(recipes[i], craftingSystem);
        for (int i = count; i < recipeEntries.Length; i++)
            recipeEntries[i].Set(null, null);
    }

    private void RefreshAllEntries()
    {
        if (recipeEntries == null) return;
        foreach (var entry in recipeEntries)
            entry.RefreshButton();
    }
}
