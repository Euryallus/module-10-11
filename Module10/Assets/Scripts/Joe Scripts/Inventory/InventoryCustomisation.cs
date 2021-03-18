using UnityEngine;
using TMPro;

public class InventoryCustomisation : PersistentObject
{
    [SerializeField] private InventorySlot customiseSlot;   //Slot for the item that will be customised NOTE: SHOULD ONLY EVER ALLOW 1 ITEM
    [SerializeField] private InventorySlot currencySlot;    //Slot for the item(s) used as currency when customising the item in the above slot
    [SerializeField] private InventorySlot resultSlot;      //Slot for the resulting customised item

    [SerializeField] private TextMeshProUGUI warningText;

    private void Awake()
    {
        customiseSlot.ItemsMovedEvent   += OnCustomiseSlotItemsMoved;
        currencySlot .ItemsMovedEvent   += OnCurrencySlotItemsMoved;
        resultSlot   .ItemsMovedEvent   += OnResultSlotItemsMoved;

        warningText.gameObject.SetActive(false);
    }

    public override void OnSave(SaveData saveData)
    {
    }

    public override void OnLoad(SaveData saveData)
    {
    }

    private void OnCustomiseSlotItemsMoved()
    {
        CheckForValidItemInputs();
    }

    private void OnCurrencySlotItemsMoved()
    {
        CheckForValidItemInputs();
    }

    private void OnResultSlotItemsMoved()
    {
    }

    private void CheckForValidItemInputs()
    {
        if(customiseSlot.ItemStack.StackSize > 0)
        {
            InventoryItem itemToCustomise = ItemManager.Instance.GetItemWithID(customiseSlot.ItemStack.StackItemsID);
            InventoryItem currencyItem = ItemManager.Instance.GetItemWithID(itemToCustomise.CustomiseItemId);

            warningText.gameObject.SetActive(true);

            if (itemToCustomise.Customisable)
            {
                int requiredCurrencyQuantity = itemToCustomise.CustomiseItemQuantity;

                if ((currencySlot.ItemStack.StackSize > 0) && (currencySlot.ItemStack.StackItemsID == currencyItem.Id))
                {

                    if (currencySlot.ItemStack.StackSize >= requiredCurrencyQuantity)
                    {
                        Debug.Log("Valid customisation setup!");
                        warningText.text = "Valid customisation setup!";
                    }
                    else
                    {
                        Debug.LogWarning("Customisation: Not enough currency. Required number is " + requiredCurrencyQuantity);
                        warningText.text = "Requires " + requiredCurrencyQuantity + "x " + currencyItem.UIName + " to customise";
                    }
                }
                else
                {
                    Debug.LogWarning("Customisation: Currency is required of type " + currencyItem.Id);
                    warningText.text = "Requires " + requiredCurrencyQuantity + "x " + currencyItem.UIName + " to customise";
                }
            }
            else
            {
                Debug.LogWarning("Customisation: Item cannot be customised");
                warningText.text = itemToCustomise.UIName + " cannot be customised";
            }
        }
        else
        {
            warningText.gameObject.SetActive(false);
        }
    }
}
