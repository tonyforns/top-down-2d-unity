using UnityEngine;

/// <summary>
/// Define una receta de crafteo: ingredientes necesarios y resultado.
/// Crear desde menú: Create → Scriptable Objects → Crafting Recipe.
/// </summary>
[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptable Objects/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Identificación")]
    [SerializeField] private string recipeName;
    [TextArea(1, 2)]
    [SerializeField] private string description;

    [Header("Ingredientes")]
    [SerializeField] private RecipeIngredient[] ingredients;

    [Header("Resultado")]
    [SerializeField] private RecipeIngredient result;

    public string RecipeName => recipeName;
    public string Description => description;
    public RecipeIngredient[] Ingredients => ingredients;
    public RecipeIngredient Result => result;

    public bool IsValid =>
        ingredients != null && ingredients.Length > 0 &&
        result.IsValid;
}
