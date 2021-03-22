using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum InventoryShowMode
{
    InventoryOnly,
    Customise,
    Craft
}

[RequireComponent(typeof(CanvasGroup))]
public class InventoryPanel : PersistentObject
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    public InventoryItemPopup                   ItemInfoPopup;

    [SerializeField] private InventorySlot[]    slots;                  //Main inventory grid
    [SerializeField] private InventorySlot      handSlot;               //Slot used to pick up and move items

    [SerializeField] private LayoutElement      customiseLayoutElement;
    [SerializeField] private CanvasGroup        customiseCanvasGroup;
    [SerializeField] private CraftingPanel      craftingPanel;

    [SerializeField] private TextMeshProUGUI    weightText;             //Text displaying how full the inventory is
    [SerializeField] private Slider             weightSlider;           //Slider that shows how close the inventory is to holding its max weight
    [SerializeField] private Image              sliderFillImage;        //Image used on the slider to show how full the inventory is
    [SerializeField] private Color              sliderStandardColour;   //Default colour of the slider image
    [SerializeField] private Color              sliderFullColour;       //Colour of the slider image when the inventory is full

    [SerializeField] private float              maxWeight;              //Maximum amount of weight this inventory can hold

    #endregion

    public InventorySlot    HandSlot    { get { return handSlot; } }
    public bool             Showing     { get { return showing; } }

    public  event Action    InventoryStateChangedEvent;     //Event that is invoked when the inventory state changes (i.e. items are added/removed/moved)
    private bool            inventoryStateChanged;          //Set to true each time an action occurs that changes the inventory's state
    private PlayerMovement  playerMovement;                 //Reference to the PlayerMovement script attached to the player character
    private CanvasGroup     canvasGroup;                    //CanvasGroup attathed to the panel
    private bool            showing;                        //Whether or not the panel is currently showing
    private float           totalWeight = 0.0f;             //The weight of all items in the inventory combined

    protected override void Start()
    {
        base.Start();

        canvasGroup     = GetComponent<CanvasGroup>();
        playerMovement  = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        //Hide the UI panel by default
        Hide();
    }

    private void Update()
    {
        //Check if the player pressed a key that should cause the panel to be shown/hidden
        CheckForShowHideInput();

        if (handSlot.ItemStack.StackSize > 0)
        {
            //When there are items in the hand slot, lerp its position to the mouse pointer
            handSlot.transform.position = Vector3.Lerp(handSlot.transform.position, Input.mousePosition, Time.unscaledDeltaTime * 20.0f);

            //Don't allow the item info popup to be shown when items are in the player's hand
            ItemInfoPopup.SetCanShow(false);
        }
        else
        {
            //No items are in the player's hand - allow the ItemInfoPopup to show info about each item in the inventory
            ItemInfoPopup.SetCanShow(true);
        }

        if (inventoryStateChanged)
        {
            //The inventory state was changed one or more times this frame
            InventoryStateChangedThisFrame();
            inventoryStateChanged = false;
        }
    }

    public override void OnSave(SaveData saveData)
    {
        Debug.Log("Saving inventory panel data");

        for (int i = 0; i < slots.Length; i++)
        {
            //Save data for each inventory slot
            saveData.AddData("slotStackSize" + i, slots[i].ItemStack.StackSize);
            saveData.AddData("stackItemsId" + i, slots[i].ItemStack.StackItemsID);
        }
    }

    public override void OnLoadSetup(SaveData saveData)
    {
        //Loading for InventoryPanel occurs in the OnLoadConfigure function since it
        //  depends on data that is initialised by other objects in the OnLoadSetup function
    }

    public override void OnLoadConfigure(SaveData saveData)
    {
        Debug.Log("Loading inventory panel data");

        for (int i = 0; i < slots.Length; i++)
        {
            //Load data for each inventory slot - the stack size and item type
            int stackSize = saveData.GetData<int>("slotStackSize" + i);
            string itemId = saveData.GetData<string>("stackItemsId" + i);

            //Add items based on the loaded values
            for (int j = 0; j < stackSize; j++)
            {
                slots[i].ItemStack.AddItemToStack(itemId, false);
            }

            //Update the UI for each slot to reflect changes
            slots[i].UpdateUI();
        }
    }

    private void CheckForShowHideInput()
    {
        //Block keyboard input if an input field is selected
        if (!CustomInputField.AnyFieldSelected)
        {
            if (!showing && Input.GetKeyDown(KeyCode.I))
            {
                Show(InventoryShowMode.InventoryOnly);
            }
            else if (showing && Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }

    public void Show(InventoryShowMode showMode)
    {
        //Customise panel should ignore UI layout unless in the Customise showMode so it doesn't take up space
        customiseLayoutElement.ignoreLayout = (showMode != InventoryShowMode.Customise);

        //Show/hide crafting and customisation panels depending on showMode
        if (showMode == InventoryShowMode.InventoryOnly)
        {
            customiseCanvasGroup.alpha = 0.0f;
            craftingPanel.Hide();
        }
        else if(showMode == InventoryShowMode.Craft)
        {
            customiseCanvasGroup.alpha = 0.0f;
            craftingPanel.Show();
        }
        else //Customise
        {
            customiseCanvasGroup.alpha = 1.0f;
            craftingPanel.Hide();
        }

        //Show inventory UI
        canvasGroup.alpha = 1.0f;
        showing = true;

        //Stop the player from moving and unlock/show the cursor so they can interact with the inventory
        playerMovement.StopMoving();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        //Hide inventory UI
        canvasGroup.alpha = 0.0f;
        showing = false;

        //Allow the player to move and lock their cursor to screen centre
        playerMovement.StartMoving();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void TryAddItemToInventory(InventoryItem item)
    {
        //Optional overload for when a bool out type is not needed
        TryAddItemToInventory(item, out bool unused);
    }

    public void TryAddItemToInventory(InventoryItem item, out bool itemAdded)
    {
        //Step 1 - loop through all slots to find valid ones

        FindValidInventorySlots(item, out int firstEmptySlot, out int firstStackableSlot);

        //Step 2: If a slot was found, add the item to it in this priority: stackable slot > empty slot

        if(firstStackableSlot == -1 && firstEmptySlot == -1)
        {
            //No empty or stackable slots, meaning the inventory is full - warn the player
            Debug.LogWarning("INVENTORY FULL!");
            itemAdded = false;
        }
        else
        {
            int chosenSlotIndex;

            if (firstStackableSlot != -1)
            {
                //Stackable slot was found, set it as the chosen slot
                chosenSlotIndex = firstStackableSlot;
            }
            else
            {
                //No stackable slots but an empty slot was found, set it as the chosen slot
                chosenSlotIndex = firstEmptySlot;
            }

            //Add the item to the chosen slot
            slots[chosenSlotIndex].ItemStack.AddItemToStack(item.Id);

            //Update slot UI to show new item
            slots[chosenSlotIndex].UpdateUI();

            itemAdded = true;
        }
    }

    private void UpdateTotalInventoryWeight()
    {
        float weight = 0.0f;

        for (int i = 0; i < slots.Length; i++)
        {
            weight += slots[i].ItemStack.StackWeight;
        }

        totalWeight = weight;

        float weightVal = totalWeight / maxWeight;

        weightSlider.value = Mathf.Clamp(weightVal, 0.0f, 1.0f);

        weightText.text = "Weight Limit (" + Mathf.FloorToInt(weightVal * 100.0f) + "%)";

        if(totalWeight >= maxWeight)
        {
            sliderFillImage.color = sliderFullColour;
            Debug.LogWarning("MAX INVENTORY WEIGHT REACHED!");
        }
        else
        {
            sliderFillImage.color = sliderStandardColour;
        }
    }

    public bool ContainsQuantityOfItem(InventoryItemGroup itemGroup, out List<InventorySlot> containingSlots)
    {
        int numberOfItemType = 0;
        containingSlots = new List<InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].ItemStack.StackSize > 0 && slots[i].ItemStack.StackItemsID == itemGroup.Item.Id)
            {
                numberOfItemType += slots[i].ItemStack.StackSize;
                containingSlots.Add(slots[i]);
            }

            if(numberOfItemType >= itemGroup.Quantity)
            {
                return true;
            }
        }

        return false;
    }

    public void InventoryStateChanged()
    {
        inventoryStateChanged = true;
    }

    private void InventoryStateChangedThisFrame()
    {
        InventoryStateChangedEvent?.Invoke();

        UpdateTotalInventoryWeight();
    }

    private void FindValidInventorySlots(InventoryItem item, out int firstEmptySlot, out int firstStackableSlot)
    {
        firstEmptySlot      = -1;   //Keeps track of the index of the first empty slot that is found
        firstStackableSlot  = -1;   //Keeps track of the index of the first slot where the item can stack that is found

        for (int i = 0; i < slots.Length; i++)
        {
            //Check if the current stack can take the item
            if (slots[i].ItemStack.CanAddItemToStack(item.Id))
            {
                if (slots[i].ItemStack.StackSize == 0 && firstEmptySlot == -1)
                {
                    //The first empty slot was found
                    firstEmptySlot = i;
                }
                else if (slots[i].ItemStack.StackSize > 0 && firstStackableSlot == -1)
                {
                    //The first stackable slot was found - no more searching is needed as stackable slots take priority
                    firstStackableSlot = i;
                    return;
                }
            }
        }
    }
}
