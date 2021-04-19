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
    [SerializeField] private PressEffectButton  buyButton;
    [SerializeField] private GameObject         buyButtonGameObj;
    [SerializeField] private TextMeshProUGUI    buyButtonText;

    [SerializeField] private Color              standardTabColour;
    [SerializeField] private Color              selectedTabColour;
    [SerializeField] private Color              standardButtonColour;
    [SerializeField] private Color              selectedButtonColour;
    [SerializeField] private Color              buyButtonColour;
    [SerializeField] private Color              cannotBuyColour;

    private InventoryPanel      inventoryPanel;
    private HotbarPanel         hotbarPanel;
    private ShopNPC             shopNPC;
    private ShopType            shopType;
    private PressEffectButton[] categoryButtons;
    private Image               selectedItemButton;
    private ShopItem            selectedItem;
    private int                 selectedCategoryIndex = -1;

    private bool                itemPurchasable;
    private bool                removeCurrencyFromHotbar;

    private const int itemsPerRow           = 5;
    private const int maxDisplayableItems   = 20;

    private void Awake()
    {
        inventoryPanel  = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();
        hotbarPanel     = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<HotbarPanel>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonLeave();
        }
    }

    public void Setup(ShopNPC npc)
    {
        shopNPC = npc;
        shopType = npc.ShopType;

        shopNameText.text = shopType.UIName;

        buyButtonGameObj.SetActive(false);

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
        selectedItemButton = null;
        buyButtonGameObj.SetActive(false);

        if (selectedCategoryIndex != -1)
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

                GameObject shopItem = Instantiate(shopItemPrefab, parent);
                Transform shopItemBG = shopItem.transform.Find("BG");
                Transform pricePanel = shopItemBG.Find("PricePanel");

                int index = i;

                shopItem.GetComponent<Button>().onClick.AddListener(delegate { SelectItemButton(shopItem.GetComponent<Image>(), category.SoldItems[index]); });

                shopItemBG.Find("ItemIcon").GetComponent<Image>().sprite = category.SoldItems[i].Item.Sprite;

                pricePanel.Find("CurrencyItem").GetComponent<Image>().sprite = category.CurrencyItem.Sprite;
                pricePanel.Find("PriceText").GetComponent<TextMeshProUGUI>().text = category.SoldItems[i].Price.ToString();
            }
            else
            {
                Debug.LogWarning("Shop category has more items than can be displayed (" + shopType.UIName + ", " + category.UIName + ")");
                break;
            }
        }
    }

    private void SelectItemButton(Image itemButton, ShopItem shopItem)
    {
        if (selectedItemButton != null)
        {
            selectedItemButton.color = standardButtonColour;
        }

        selectedItemButton = itemButton;

        selectedItemButton.color = selectedButtonColour;

        SelectItem(shopItem);
    }

    private void SelectItem(ShopItem shopItem)
    {
        if(shopItem != null)
        {
            selectedItem = shopItem;

            itemPurchasable = CanPlayerPurchaseItem(shopItem);

            buyButtonGameObj.SetActive(true);
            buyButtonText.text = "Buy " + shopItem.Item.UIName + " for " + shopItem.Price + " " + shopType.Categories[selectedCategoryIndex].CurrencyItem.UIName;

            if(itemPurchasable)
            {
                buyButton.SetButtonColour(buyButtonColour);
            }
            else
            {
                buyButton.SetButtonColour(cannotBuyColour);
            }
        }
        else
        {
            selectedItem = null;

            buyButtonGameObj.SetActive(false);
        }
    }

    private bool CanPlayerPurchaseItem(ShopItem shopItem)
    {
        Item    itemRequired        = shopType.Categories[selectedCategoryIndex].CurrencyItem;
        int     quantityRequired    = shopItem.Price;

        ItemGroup requiredGroup = new ItemGroup(itemRequired, quantityRequired);

        if (inventoryPanel.ContainsQuantityOfItem(requiredGroup))
        {
            removeCurrencyFromHotbar = false;
            return true;
        }
        else if (hotbarPanel.ContainsQuantityOfItem(requiredGroup))
        {
            removeCurrencyFromHotbar = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ButtonBuy()
    {
        if (itemPurchasable)
        {
            if(removeCurrencyFromHotbar)
            {
                for (int i = 0; i < selectedItem.Price; i++)
                {
                    hotbarPanel.RemoveItemFromHotbar(shopType.Categories[selectedCategoryIndex].CurrencyItem);
                }
            }
            else
            {
                for (int i = 0; i < selectedItem.Price; i++)
                {
                    inventoryPanel.RemoveItemFromInventory(shopType.Categories[selectedCategoryIndex].CurrencyItem);
                }
            }

            inventoryPanel.AddItemToInventory(selectedItem.Item);

            SelectItem(selectedItem);
        }
        else
        {
            NotificationManager.Instance.ShowNotification(NotificationTextType.CantAffordItem,
                new string[] { selectedItem.Price.ToString(), shopType.Categories[selectedCategoryIndex].CurrencyItem.UIName });
        }
    }

    public void ButtonLeave()
    {
        shopNPC.StopInteracting();

        Destroy(gameObject);
    }
}
