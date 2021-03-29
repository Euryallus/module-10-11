using UnityEngine;
using TMPro;

public class InventoryCustomisation : MonoBehaviour, IPersistentObject
{
    [SerializeField] private ItemContainer      inventoryItemContainer;
    [SerializeField] private ContainerSlotUI    customiseSlotUI;             //Slot for the item that will be customised NOTE: SHOULD ONLY EVER ALLOW 1 ITEM
    [SerializeField] private ContainerSlotUI    currencySlotUI;              //Slot for the item(s) used as currency when customising the item in the above slot
    [SerializeField] private ContainerSlotUI    resultSlotUI;                //Slot for the resulting customised item
    
    [SerializeField] private GameObject      customisationOptionsPanel;
    [SerializeField] private TMP_InputField  customNameInput;
    
    [SerializeField] private TextMeshProUGUI warningText;

    private ItemManager     itemManager;
    private string          customisedItemName;
    private ContainerSlot   customiseSlot;
    private ContainerSlot   currencySlot;
    private ContainerSlot   resultSlot;

    private void Awake()
    {
        customiseSlot   = new ContainerSlot(1, inventoryItemContainer);
        currencySlot    = new ContainerSlot(0, inventoryItemContainer);
        resultSlot      = new ContainerSlot(0, inventoryItemContainer);

        customiseSlotUI .LinkToContainerSlot(customiseSlot);
        currencySlotUI  .LinkToContainerSlot(currencySlot);
        resultSlotUI    .LinkToContainerSlot(resultSlot);

        customiseSlot.ItemsMovedEvent   += OnCustomiseSlotItemsMoved;
        currencySlot .ItemsMovedEvent   += OnCurrencySlotItemsMoved;
        resultSlot   .ItemsMovedEvent   += OnResultSlotItemsMoved;

        customisationOptionsPanel.SetActive(false);
    }

    protected void Start()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            += OnSave;
        slm.LoadObjectsSetupEvent       += OnLoadSetup;
        slm.LoadObjectsConfigureEvent   += OnLoadConfigure;

        itemManager = ItemManager.Instance;

        ShowDefaultWarningText();
    }

    public void OnSave(SaveData saveData)
    {
        saveData.AddData("customiseStackItemId", customiseSlot.ItemStack.StackSize > 0 ? customiseSlot.ItemStack.StackItemsID : "");

        saveData.AddData("currencyStackSize", currencySlot.ItemStack.StackSize);
        saveData.AddData("currencyStackItemId", currencySlot.ItemStack.StackItemsID);

        saveData.AddData("resultStackItemId", resultSlot.ItemStack.StackSize > 0 ? resultSlot.ItemStack.StackItemsID : "");
    }

    public void OnLoadSetup(SaveData saveData)
    {
    }

    public void OnLoadConfigure(SaveData saveData)
    {
        //Load any item that was in the customise slot
        string customiseItemId = saveData.GetData<string>("customiseStackItemId");
        if (!string.IsNullOrEmpty(customiseItemId))
        {
            customiseSlot.ItemStack.AddItemToStack(customiseItemId, false);
        }

        //Load any items that were in the currency slot
        int currencyStackSize = saveData.GetData<int>("currencyStackSize");
        string currencyItemId = saveData.GetData<string>("currencyStackItemId");
        for (int i = 0; i < currencyStackSize; i++)
        {
            currencySlot.ItemStack.AddItemToStack(currencyItemId, false);
        }

        //Load any that that was in the result slot
        string resultItemId = saveData.GetData<string>("resultStackItemId");
        if (!string.IsNullOrEmpty(resultItemId))
        {
            resultSlot.ItemStack.AddItemToStack(resultItemId, false);
        }

        //Update all UI based on loaded values
        customiseSlotUI.UpdateUI();
        currencySlotUI.UpdateUI();
        ItemInputChanged();
    }

    private void OnCustomiseSlotItemsMoved()
    {
        //The player added/removed an item from the customise slot, update UI based on the new input
        ItemInputChanged();
    }

    private void OnCurrencySlotItemsMoved()
    {
        //The player added/removed item(s) from the currency slot, update UI based on the new input
        ItemInputChanged();
    }

    private void OnResultSlotItemsMoved()
    {
        //The player took the resulting item
        //Increment the custom item id so the next custom item that is created will have a unique id
        itemManager.IncrementUniqueCustomItemId();

        customiseSlot.ItemStack.TryRemoveItemFromStack();

        Item customiseSlotItem = itemManager.GetItemWithID(customiseSlot.ItemStack.StackItemsID);

        for (int i = 0; i < customiseSlotItem.CurrencyItemQuantity; i++)
        {
            currencySlot.ItemStack.TryRemoveItemFromStack();
        }

        customiseSlotUI.UpdateUI();
        currencySlotUI.UpdateUI();

        ItemInputChanged();
    }

    private void ItemInputChanged()
    {
        //Remove any existing items in the result slot
        resultSlot.ItemStack.TryRemoveItemFromStack();

        //Get the unique id that will be used for the custom item being added/removed
        string customItemId = itemManager.GetUniqueCustomItemId();

        //If the player has put a valid combination of items into the customise/currency slots, show the output item
        if (CheckForValidItemInputs())
        {
            customisationOptionsPanel.SetActive(true);

            Item baseItem = itemManager.GetItemWithID(customiseSlot.ItemStack.StackItemsID);

            //By default, the new 'result' item will be based on the item in the customise slot. However, if the item in the customise slot is already
            //  a custom item, the resulting item should be based on the original base item (i.e. the non-custom item at the top of the heirarchy)
            string originalBaseItemId = baseItem.Id;
            if (baseItem.CustomItem)
            {
                originalBaseItemId = baseItem.BaseItemId;
            }

            //Add a new custom item; this will be the resulting item type that the player can take
            itemManager.AddCustomItem(customItemId, originalBaseItemId);

            SetCustomisedItemName(itemManager.GetItemWithID(customiseSlot.ItemStack.StackItemsID).UIName);

            resultSlot.ItemStack.AddItemToStack(customItemId, false);
        }
        else
        {
            customisationOptionsPanel.SetActive(false);

            itemManager.RemoveCustomItem(customItemId);
        }

        //Update the result slot UI to reflect changes
        resultSlotUI.UpdateUI();
    }

    private bool CheckForValidItemInputs()
    {
        if (customiseSlot.ItemStack.StackSize > 0)
        {
            Item itemToCustomise = itemManager.GetItemWithID(customiseSlot.ItemStack.StackItemsID);

            if (itemToCustomise.Customisable)
            {
                Item currencyItem = null;
                int requiredCurrencyQuantity = 0;

                if(itemToCustomise.CurrencyItemQuantity > 0)
                {
                    currencyItem = itemManager.GetItemWithID(itemToCustomise.CurrencyItemId);
                    requiredCurrencyQuantity = itemToCustomise.CurrencyItemQuantity;
                }

                if ((currencySlot.ItemStack.StackSize >= requiredCurrencyQuantity) && (currencyItem == null || currencySlot.ItemStack.StackItemsID == currencyItem.Id))
                {
                    warningText.text = "<color=#464646>Customising " + itemToCustomise.UIName + ".</color>";
                    return true;

                }
                else
                {
                    warningText.text = "Requires " + requiredCurrencyQuantity + "x " + currencyItem.UIName + " to customise.";
                    return false;
                }
            }
            else
            {
                warningText.text = itemToCustomise.UIName + " cannot be customised.";
                return false;
            }
        }
        else
        {
            ShowDefaultWarningText();
            return false;
        }
    }

    private void ShowDefaultWarningText()
    {
        warningText.text = "<color=#464646>Place an item in the first slot to customise it.</color>";
    }

    public void OnCustomNameInputChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            SetCustomisedItemName(value);
        }
        else
        {
            SetCustomisedItemName(itemManager.GetItemWithID(customiseSlot.ItemStack.StackItemsID).UIName);
        }
    }

    private void SetCustomisedItemName(string name)
    {
        customisedItemName = name;
        customNameInput.text = customisedItemName;

        itemManager.SetCustomGenericItemData(itemManager.GetUniqueCustomItemId(), customisedItemName);
    }
}