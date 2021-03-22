using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftingItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image              backgroundImage;
    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemQuantityContainer;
    [SerializeField] private TextMeshProUGUI    itemQuantityText;

    [SerializeField] private Color              standardColour;
    [SerializeField] private Color              selectedColour;

    private CraftingPanel   parentPanel;
    private CraftingRecipe  recipe;

    public void Setup(CraftingPanel parentPanel, CraftingRecipe recipe)
    {
        this.parentPanel    = parentPanel;
        this.recipe         = recipe;

        itemImage.sprite = recipe.ResultItem.Item.Sprite;

        itemQuantityContainer.SetActive(recipe.ResultItem.Quantity > 1);
        itemQuantityText.text = recipe.ResultItem.Quantity.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentPanel.Showing)
        {
            parentPanel.InventoryPanel.ItemInfoPopup.ShowPopup(recipe.ResultItem.Item.Id);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        parentPanel.InventoryPanel.ItemInfoPopup.HidePopup();
    }

    public void OnClick()
    {
        if(parentPanel.SelectedRecipe != recipe)
        {
            parentPanel.SelectRecipe(recipe, this);
        }
        else
        {
            parentPanel.SelectRecipe(null, null);
        }
    }

    public void Select()
    {
        backgroundImage.color = selectedColour;
    }

    public void Deselect()
    {
        backgroundImage.color = standardColour;
    }
}
