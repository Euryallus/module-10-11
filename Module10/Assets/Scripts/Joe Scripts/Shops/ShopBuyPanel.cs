using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopBuyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI    shopNameText;
    [SerializeField] private Transform[]        itemRows;
    [SerializeField] private Transform          categoriesParent;
    [SerializeField] private GameObject         shopItemPrefab;
    [SerializeField] private GameObject         shopCategoryPrefab;

    [SerializeField] private Color              standardTabColour;
    [SerializeField] private Color              selectedTabColour;

    private ShopNPC             shopNPC;
    private ShopType            shopType;
    private PressEffectButton[] categoryButtons;
    private int                 selectedCategoryIndex = -1;

    private const int itemsPerRow           = 5;
    private const int maxDisplayableItems   = 20;

    public void Setup(ShopNPC npc)
    {
        shopNPC = npc;
        shopType = npc.ShopType;

        shopNameText.text = shopType.UIName;

        categoryButtons = new PressEffectButton[shopType.Categories.Length];

        for (int i = 0; i < shopType.Categories.Length; i++)
        {
            GameObject categoryButton = Instantiate(shopCategoryPrefab, categoriesParent).transform.GetChild(0).gameObject;

            categoryButtons[i] = categoryButton.GetComponent<PressEffectButton>();

            categoryButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = shopType.Categories[i].UIName;

            int categoryIndex = i;

            categoryButton.GetComponent<Button>().onClick.AddListener(delegate { SelectCategory(categoryIndex); });
        }

        SelectCategory(0);
    }

    private void SelectCategory(int categoryIndex)
    {
        if(selectedCategoryIndex != -1)
        {
            categoryButtons[selectedCategoryIndex].SetButtonColour(standardTabColour);
        }

        selectedCategoryIndex = categoryIndex;

        categoryButtons[categoryIndex].SetButtonColour(selectedTabColour);

        ShopCategory selectedCategory = shopNPC.ShopType.Categories[categoryIndex];


        SetupCategoryUI(selectedCategory);
    }

    private void SetupCategoryUI(ShopCategory category)
    {
        for (int i = 0; i < itemRows.Length; i++)
        {
            //Remove all existing item buttons
            foreach(Transform t in itemRows[i].transform)
            {
                Destroy(t.gameObject);
            }

            //Hide all item rows by default
            itemRows[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < category.SoldItems.Length; i++)
        {
            if (i < maxDisplayableItems)
            {
                Transform parent = itemRows[i / itemsPerRow];

                parent.gameObject.SetActive(true);

                Transform shopItemBG = Instantiate(shopItemPrefab, parent).transform.Find("BG");
                Transform pricePanel = shopItemBG.Find("PricePanel");

                shopItemBG.Find("ItemIcon").GetComponent<Image>().sprite = category.SoldItems[i].Item.Sprite;

                pricePanel.Find("CurrencyItem").GetComponent<Image>().sprite = category.CurrencyItem.Sprite;
                pricePanel.Find("PriceText").GetComponent<TextMeshProUGUI>().text = category.SoldItems[i].Price.ToString();
            }
            else
            {
                break;
            }
        }
    }

    public void ButtonLeave()
    {
        shopNPC.StopInteracting();

        Destroy(gameObject);
    }
}
