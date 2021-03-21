using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingItemPreview : MonoBehaviour
{
    [SerializeField] private Image              backgroundImage;
    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemQuantityContainer;
    [SerializeField] private TextMeshProUGUI    itemQuantityText;
    [SerializeField] private GameObject         itemWarning;

    [SerializeField] private Color validColour;
    [SerializeField] private Color invalidColour;

    public void Setup(bool valid, InventoryItemGroup itemGroup)
    {
        if (valid)
        {
            itemWarning.SetActive(false);
            backgroundImage.color = validColour;
        }
        else
        {
            itemWarning.SetActive(true);
            backgroundImage.color = invalidColour;
        }

        itemImage.sprite = itemGroup.Item.Sprite;

        itemQuantityContainer.SetActive(itemGroup.Quantity > 1);
        itemQuantityText.text = itemGroup.Quantity.ToString();
    }
}
