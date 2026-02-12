using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Representa un solo slot del inventario en la UI: icono + cantidad.
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    public void Set(ItemStack stack)
    {
        if (stack.IsEmpty)
        {
            if (iconImage != null)
            {
                iconImage.enabled = false;
                iconImage.sprite = null;
            }
            if (amountText != null)
                amountText.gameObject.SetActive(false);
            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = stack.Data.Icon;
        }

        if (amountText != null)
        {
            bool showCount = stack.Data.IsStackable && stack.Amount > 1;
            amountText.gameObject.SetActive(showCount);
            if (showCount)
                amountText.text = stack.Amount.ToString();
        }
    }
}
