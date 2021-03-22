using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class CraftingPanel : MonoBehaviour
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    public InventoryPanel InventoryPanel;

    [SerializeField] private GameObject         prefabCraftingItemButton;   //Prefab for buttons used to select which item to craft
    [SerializeField] private GameObject         prefabRequiredItemPreview;  //Prefab for UI that displays a required item for the crafting recipe

    [SerializeField] private Transform          craftingItemsContent;       //Parent transform for crafting buttons
    [SerializeField] private GameObject         requiredItemsPanel;         //Panel displaying required items
    [SerializeField] private Transform          requiredItemsContent;       //Parent transform for required item previews
    [SerializeField] private PressEffectButton  craftButton;                //Button that crafts the selected item when clicked
    [SerializeField] private TextMeshProUGUI    craftButtonText;            //Text displayed on the above button

    [SerializeField] private Color              validCraftColour;           //Colour to use for UI when an item can be crafted
    [SerializeField] private Color              invalidCraftColour;         //Colour to use for UI when an item cannot be crafted

    #endregion

    #region Properties
    public CraftingRecipe   SelectedRecipe  { get { return selectedRecipe; } }
    public bool             Showing         { get { return showing; } }

    #endregion

    private CanvasGroup                 canvasGroup;
    private bool                        showing;
    private CraftingRecipe              selectedRecipe;             //The currently selected crafting recipe
    private CraftingItemButton          selectedButton;             //The button corresponding to the selected recipe
    private List<InventorySlot>[]       slotsContainingRecipeItems; //An array containing lists on inventory slots, the array index corresponds to the index of
                                                                    //  the recipe item that requires items from the slot(s) that are in the list to be crafted


    private void Awake()
    {
        InventoryPanel.ItemContainer.ContainerStateChangedEvent += CheckForValidCraftingSetup;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        //Deselect all recipes by default
        SelectRecipe(null, null);

        CraftingRecipe[] craftingRecipes = ItemManager.Instance.CraftingRecipes;

        //Create a crafting item button for each crafting recipe in the item manager
        for (int i = 0; i < craftingRecipes.Length; i++)
        {
            GameObject craftingItemButton = Instantiate(prefabCraftingItemButton, craftingItemsContent);
            craftingItemButton.GetComponent<CraftingItemButton>().Setup(this, craftingRecipes[i]);
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        showing = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        showing = false;
    }

    public void SelectRecipe(CraftingRecipe recipe, CraftingItemButton button)
    {
        //Deselect the currently selected button, if there is one
        if(selectedButton != null)
        {
            selectedButton.Deselect();
        }

        //Select the button corresponding to the new recipe
        if(button != null)
        {
            button.Select();
            selectedButton = button;
        }

        if (recipe != null)
        {
            //Recipe is not null, i.e. a recipe was selected
            selectedRecipe = recipe;

            InventoryItemGroup resultItem = recipe.ResultItem;

            //Update craft button text based on the result item and quantity that will be crafted
            craftButtonText.text = "Craft " + (resultItem.Quantity > 1 ? (resultItem.Quantity + "x ") : "") + resultItem.Item.UIName;

            //Update crafting panel state to show the player if they can currently craft the result item
            CheckForValidCraftingSetup();

            //Show the panel displaying required items and the button used to craft the selected result item
            requiredItemsPanel                      .SetActive(true);
            craftButton.transform.parent.gameObject .SetActive(true);
        }
        else
        {
            //Recipe is null, i.e. no button/recipe is now selected
            selectedRecipe = null;

            //Hide the panel displaying required items and the crafting button since nothing is selected
            requiredItemsPanel                      .SetActive(false);
            craftButton.transform.parent.gameObject .SetActive(false);
        }
    }

    private void CheckForValidCraftingSetup()
    {
        if(selectedRecipe != null)
        {
            List<InventoryItemGroup> requiredItems = selectedRecipe.RecipeItems;

            //Destroy any existing item previews to make way for the updated ones
            foreach (Transform requiredItemTransform in requiredItemsContent.transform)
            {
                Destroy(requiredItemTransform.gameObject);
            }

            bool requiredItemsAreInInventory = true;    //Keeps track of whether the player has all required items in their inventory

            slotsContainingRecipeItems = new List<InventorySlot>[requiredItems.Count];  //Keeps track of the slots that contain the required items

            for (int i = 0; i < requiredItems.Count; i++)
            {
                //Create a new item preview for each of the required items
                GameObject itemPreviewGameObject = Instantiate(prefabRequiredItemPreview, requiredItemsContent);

                if (InventoryPanel.ItemContainer.ContainsQuantityOfItem(requiredItems[i], out List<InventorySlot> containingSlots))
                {
                    //The player has the necessary amount of the current item in their inventory, show that on the preview
                    itemPreviewGameObject.GetComponent<CraftingItemPreview>().Setup(true, requiredItems[i]);
                    slotsContainingRecipeItems[i] = containingSlots;
                }
                else
                {
                    //The player does not have the necessary amount of the current item in their inventory, show that on the preview
                    itemPreviewGameObject.GetComponent<CraftingItemPreview>().Setup(false, requiredItems[i]);
                    requiredItemsAreInInventory = false;
                }
            }

            //Make the button interactable only if the player has all the required items
            craftButton.SetInteractable(requiredItemsAreInInventory);

            //Change the button colour depending on if the player has all the required items
            if (requiredItemsAreInInventory)
            {
                craftButton.SetButtonColour(validCraftColour);
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
            //Craft the selected item when the craft button is clicked
            CraftSelectedResultItem();
        }
        else
        {
            Debug.LogError("Player should not be able to press craft button with null recipe");
        }
    }

    private void CraftSelectedResultItem()
    {
        //Loop through all recipe items to be removed from the player's inventory
        for (int i = 0; i < selectedRecipe.RecipeItems.Count; i++)
        {
            int removeSlotIndex = 0;    //Keeps track of the index of the slot containing the recipe item(s)

            for (int j = 0; j < selectedRecipe.RecipeItems[i].Quantity; j++)
            {
                InventorySlot currentRecipeItemSlot = slotsContainingRecipeItems[i][removeSlotIndex];

                //Remove the recipe item from the stack in the target slot and update the slot UI to reflect changes
                currentRecipeItemSlot.ItemStack.TryRemoveItemFromStack();
                currentRecipeItemSlot.UpdateUI();

                if (currentRecipeItemSlot.ItemStack.StackSize == 0)
                {
                    //The current slot no longer contains any of the required item, move to the next valid one
                    removeSlotIndex++;
                }
            }
        }

        for (int i = 0; i < SelectedRecipe.ResultItem.Quantity; i++)
        {
            //Attempt to add the crafted item to the player's inventory
            InventoryPanel.ItemContainer.TryAddItemToContainer(SelectedRecipe.ResultItem.Item, out bool itemAdded);

            if (itemAdded)
            {
                //Successfully added item to inventory
            }
            else
            {
                //Inventory too full to add item. TODO: Drop item instead
                Debug.LogWarning("INVENTORY TOO FULL FOR ITEM. ITEM SHOULD BE DROPPED INSTEAD");
            }
        }
    }
}
