using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingPanel : MonoBehaviour
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    public InventoryPanel InventoryPanel;

    [SerializeField] private GameObject         prefabCraftingItemButton;
    [SerializeField] private GameObject         prefabRequiredItemPreview;

    [SerializeField] private Transform          craftingItemsContent;
    [SerializeField] private GameObject         requiredItemsPanel;
    [SerializeField] private Transform          requiredItemsContent;
    [SerializeField] private PressEffectButton  craftButton;
    [SerializeField] private TextMeshProUGUI    craftButtonText;

    [SerializeField] private Color              validCraftColour;
    [SerializeField] private Color              invalidCraftColour;

    #endregion

    public CraftingRecipe SelectedRecipe { get { return selectedRecipe; } }

    private CraftingRecipe              selectedRecipe;
    private CraftingItemButton          selectedButton;
    private List<InventorySlot>[]       slotsContainingRecipeItems;

    private void Awake()
    {
        InventoryPanel.InventoryStateChangedEvent += CheckForValidCraftingSetup;
    }

    private void Start()
    {
        SelectRecipe(null, null);

        CraftingRecipe[] craftingRecipes = ItemManager.Instance.CraftingRecipes;

        for (int i = 0; i < craftingRecipes.Length; i++)
        {
            GameObject craftingItemButton = Instantiate(prefabCraftingItemButton, craftingItemsContent);
            craftingItemButton.GetComponent<CraftingItemButton>().Setup(this, craftingRecipes[i]);
        }
    }

    public void SelectRecipe(CraftingRecipe recipe, CraftingItemButton button)
    {
        if(selectedButton != null)
        {
            selectedButton.Deselect();
        }
        if(button != null)
        {
            button.Select();
            selectedButton = button;
        }

        if (recipe != null)
        {
            selectedRecipe = recipe;

            InventoryItemGroup resultItem = recipe.ResultItem;

            craftButtonText.text = "Craft " + (resultItem.Quantity > 1 ? (resultItem.Quantity + "x ") : "") + resultItem.Item.UIName;

            CheckForValidCraftingSetup();

            requiredItemsPanel                      .SetActive(true);
            craftButton.transform.parent.gameObject .SetActive(true);
        }
        else
        {
            selectedRecipe = null;

            requiredItemsPanel                      .SetActive(false);
            craftButton.transform.parent.gameObject .SetActive(false);
        }
    }

    private void CheckForValidCraftingSetup()
    {
        if(selectedRecipe != null)
        {
            List<InventoryItemGroup> requiredItems = selectedRecipe.RecipeItems;

            foreach (Transform requiredItemTransform in requiredItemsContent.transform)
            {
                Destroy(requiredItemTransform.gameObject);
            }

            bool requiredItemsAreInInventory = true;
            slotsContainingRecipeItems = new List<InventorySlot>[requiredItems.Count];

            for (int i = 0; i < requiredItems.Count; i++)
            {
                GameObject itemPreviewGameObject = Instantiate(prefabRequiredItemPreview, requiredItemsContent);


                if (InventoryPanel.ContainsQuantityOfItem(requiredItems[i], out List<InventorySlot> containingSlots))
                {
                    itemPreviewGameObject.GetComponent<CraftingItemPreview>().Setup(true, requiredItems[i]);
                    slotsContainingRecipeItems[i] = containingSlots;
                }
                else
                {
                    itemPreviewGameObject.GetComponent<CraftingItemPreview>().Setup(false, requiredItems[i]);
                    requiredItemsAreInInventory = false;
                }
            }

            craftButton.SetInteractable(requiredItemsAreInInventory);

            if (requiredItemsAreInInventory)
            {
                craftButton.SetButtonColour(validCraftColour);

                //Debug.Log("===== SLOTS: =====");
                //for (int i = 0; i < slotsContainingRequiredItems.Length; i++)
                //{
                //    string s = "Item " + i + " in slot(s): ";
                //    for (int j = 0; j < slotsContainingRequiredItems[i].Count; j++)
                //    {
                //        s += slotsContainingRequiredItems[i][j] + " ";
                //    }
                //    Debug.Log(s);
                //}
            }
            else
            {
                craftButton.SetButtonColour(invalidCraftColour);
            }
        }
    }

    public void CraftButtonClick()
    {
        if(selectedRecipe != null)
        {
            CraftSelectedResultItem();
        }
        else
        {
            Debug.LogError("Player should not be able to press craft button with null recipe");
        }
    }

    private void CraftSelectedResultItem()
    {
        Debug.Log("CRAFTING " + selectedRecipe.ResultItem.Item.UIName);

        for (int i = 0; i < selectedRecipe.RecipeItems.Count; i++)
        {
            int removeSlotIndex = 0;

            for (int j = 0; j < selectedRecipe.RecipeItems[i].Quantity; j++)
            {
                InventorySlot currentRecipeItemSlot = slotsContainingRecipeItems[i][removeSlotIndex];

                currentRecipeItemSlot.ItemStack.TryRemoveItemFromStack();
                currentRecipeItemSlot.UpdateUI();

                if (currentRecipeItemSlot.ItemStack.StackSize == 0)
                {
                    removeSlotIndex++;
                }
            }
        }

        for (int i = 0; i < SelectedRecipe.ResultItem.Quantity; i++)
        {
            InventoryPanel.TryAddItemToInventory(SelectedRecipe.ResultItem.Item, out bool itemAdded);

            if (itemAdded)
            {
                //Successfully added item
            }
            else
            {
                //Inventory too full to add item. TODO: Drop item instead
                Debug.LogWarning("INVENTORY TOO FULL FOR ITEM. ITEM SHOULD BE DROPPED INSTEAD");
            }
        }
    }
}
