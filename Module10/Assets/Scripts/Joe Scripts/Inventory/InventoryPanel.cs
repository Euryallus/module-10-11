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

public class InventoryPanel : UIPanel
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private List<ContainerSlotUI>  slotsUI;
    [SerializeField] private ItemContainer          itemContainer;

    [SerializeField] private LayoutElement          customiseLayoutElement;
    [SerializeField] private CanvasGroup            customiseCanvasGroup;
    [SerializeField] private CraftingPanel          craftingPanel;

    [SerializeField] private TextMeshProUGUI        weightText;             //Text displaying how full the inventory is
    [SerializeField] private Slider                 weightSlider;           //Slider that shows how close the inventory is to holding its max weight
    [SerializeField] private Image                  sliderFillImage;        //Image used on the slider to show how full the inventory is
    [SerializeField] private Color                  sliderStandardColour;   //Default colour of the slider image
    [SerializeField] private Color                  sliderFullColour;       //Colour of the slider image when the inventory is full

    [SerializeField] private float                  maxWeight;              //Maximum amount of weight this inventory can hold

    #endregion

    public ItemContainer    ItemContainer   { get { return itemContainer; } }

    private PlayerMovement  playerMovement;                 //Reference to the PlayerMovement script attached to the player character
    private float           totalWeight = 0.0f;             //The weight of all items in the inventory combined
    private HandSlotUI      handSlotUI;

    private void Awake()
    {
        //Update inventory weight when an item is added/removed/moved in the container
        itemContainer.ContainerStateChangedEvent += UpdateTotalInventoryWeight;
    }

    protected override void Start()
    {
        base.Start();

        playerMovement  = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        itemContainer.LinkSlotsToUI(slotsUI);

        handSlotUI = GameObject.FindGameObjectWithTag("HandSlot").GetComponent<HandSlotUI>();

        //Hide the UI panel by default
        Hide();
    }

    private void Update()
    {
        //Check if the player pressed a key that should cause the panel to be shown/hidden
        CheckForShowHideInput();

        if (handSlotUI.Slot.ItemStack.StackSize > 0)
        {
            //When there are items in the hand slot, lerp its position to the mouse pointer
            handSlotUI.transform.position = Vector3.Lerp(handSlotUI.transform.position, Input.mousePosition, Time.unscaledDeltaTime * 20.0f);

            //Don't allow the item info popup to be shown when items are in the player's hand
            itemContainer.ItemInfoPopup.SetCanShow(false);
        }
        else
        {
            //No items are in the player's hand - allow the ItemInfoPopup to show info about each item in the inventory
            itemContainer.ItemInfoPopup.SetCanShow(true);
        }
    }

    public void DebugAddItemButton(Item item)
    {
        AddItemToInventory(item);
    }

    public void DebugRemoveItemButton(Item item)
    {
        RemoveItemFromInventory(item);
    }

    public void AddItemToInventory(Item item)
    {
        itemContainer.TryAddItemToContainer(item);
    }

    public void AddItemToInventory(string itemId)
    {
        ItemContainer.TryAddItemToContainer(ItemManager.Instance.GetItemWithID(itemId));
    }

    public bool RemoveItemFromInventory(Item item)
    {
        return RemoveItemFromInventory(item.Id);
    }

    public bool RemoveItemFromInventory(string itemId)
    {
        return itemContainer.TryRemoveItemFromContainer(itemId);
    }

    public bool ContainsQuantityOfItem(ItemGroup itemGroup)
    {
        return itemContainer.ContainsQuantityOfItem(itemGroup, out _);
    }

    private void CheckForShowHideInput()
    {
        //Block keyboard input if an input field is selected
        if (!CustomInputField.AnyFieldSelected)
        {
            if (!showing && Input.GetKeyDown(KeyCode.I) && playerMovement.GetCanMove())
            {
                //Show the inventory if p the player presses I when it's not already showing and they can move (i.e. not in another menu)
                Show(InventoryShowMode.InventoryOnly);
            }
            else if (showing && (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape)))
            {
                Hide();
            }
        }
    }

    public void Show(InventoryShowMode showMode, float yOffset = 30.0f)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, yOffset);

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

        base.Show();

        //Stop the player from moving and unlock/show the cursor so they can interact with the inventory
        playerMovement.StopMoving();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Show()
    {
        Debug.LogError("Should not use default Show function for InventoryPanel - use overload that takes InventoryShowMode and y-offset instead");
    }

    public override void Hide()
    {
        base.Hide();

        //Allow the player to move and lock their cursor to screen centre
        playerMovement.StartMoving();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UpdateTotalInventoryWeight()
    {
        float weight = 0.0f;

        for (int i = 0; i < itemContainer.Slots.Length; i++)
        {
            weight += itemContainer.Slots[i].ItemStack.StackWeight;
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
}
