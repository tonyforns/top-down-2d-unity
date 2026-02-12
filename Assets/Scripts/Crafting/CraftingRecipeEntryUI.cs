using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Una fila de la lista de crafteo: nombre, resultado (icono + cantidad) y botón Craft.
/// Opcionalmente muestra ingredientes. Refresca el botón según CanCraft.
/// </summary>
public class CraftingRecipeEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image resultIcon;
    [SerializeField] private TMP_Text resultAmountText;
    [SerializeField] private Button craftButton;
    [Header("Opcional: lista de ingredientes")]
    [SerializeField] private Image[] ingredientIcons;
    [SerializeField] private TMP_Text[] ingredientAmountTexts;

    private CraftingRecipe _recipe;
    private CraftingSystem _system;

    private void Awake()
    {
        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftClicked);
    }

    /// <summary>Asigna receta y sistema; actualiza la vista y el estado del botón.</summary>
    public void Set(CraftingRecipe recipe, CraftingSystem system)
    {
        _recipe = recipe;
        _system = system;
        RefreshVisuals();
        RefreshButton();
    }

    /// <summary>Llamar cuando cambia el inventario para actualizar si se puede craftar.</summary>
    public void RefreshButton()
    {
        if (craftButton != null)
            craftButton.interactable = _system != null && _recipe != null && _system.CanCraft(_recipe);
    }

    private void RefreshVisuals()
    {
        if (_recipe == null) return;

        if (nameText != null)
            nameText.text = _recipe.RecipeName;

        var result = _recipe.Result;
        if (resultIcon != null)
        {
            resultIcon.enabled = result.Item != null;
            if (result.Item != null)
                resultIcon.sprite = result.Item.Icon;
        }
        if (resultAmountText != null)
        {
            resultAmountText.gameObject.SetActive(result.Amount > 1);
            resultAmountText.text = result.Amount.ToString();
        }

        var ingredients = _recipe.Ingredients;
        if (ingredients == null) return;
        for (int i = 0; i < ingredients.Length && (ingredientIcons == null || i < ingredientIcons.Length); i++)
        {
            var ing = ingredients[i];
            if (ingredientIcons != null && i < ingredientIcons.Length && ingredientIcons[i] != null)
            {
                ingredientIcons[i].enabled = ing.Item != null;
                if (ing.Item != null)
                    ingredientIcons[i].sprite = ing.Item.Icon;
            }
            if (ingredientAmountTexts != null && i < ingredientAmountTexts.Length && ingredientAmountTexts[i] != null)
            {
                ingredientAmountTexts[i].text = ing.Amount.ToString();
                ingredientAmountTexts[i].gameObject.SetActive(ing.Item != null);
            }
        }
    }

    private void OnCraftClicked()
    {
        if (_system == null || _recipe == null) return;
        if (_system.Craft(_recipe))
            RefreshButton();
    }
}
