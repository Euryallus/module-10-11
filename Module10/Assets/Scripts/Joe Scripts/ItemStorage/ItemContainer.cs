using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : PersistentObject
{
    public ItemInfoPopup ItemInfoPopup;

    [SerializeField] private ContainerSlot[]    slots;                  //Main inventory grid
    [SerializeField] private ContainerSlot      handSlot;               //Slot used to pick up and move items

    public ContainerSlot[]  Slots       { get { return slots; } }
    public ContainerSlot    HandSlot    { get { return handSlot; } }

    public event Action ContainerStateChangedEvent;     //Event that is invoked when the container state changes (i.e. items are added/removed/moved)
    private bool        containerStateChanged;          //Set to true each time an action occurs that changes the item container's state

    private void Update()
    {
        if (containerStateChanged)
        {
            //The container state was changed one or more times this frame
            ContainerStateChangedThisFrame();
            containerStateChanged = false;
        }
    }

    public void ContainerStateChanged()
    {
        containerStateChanged = true;
    }

    private void ContainerStateChangedThisFrame()
    {
        ContainerStateChangedEvent?.Invoke();
    }

    public override void OnSave(SaveData saveData)
    {
        Debug.Log("Saving item containerdata");

        for (int i = 0; i < slots.Length; i++)
        {
            //Save data for each container slot
            saveData.AddData("slotStackSize" + i, slots[i].ItemStack.StackSize);
            saveData.AddData("stackItemsId" + i, slots[i].ItemStack.StackItemsID);
        }
    }

    public override void OnLoadSetup(SaveData saveData)
    {
        //Loading for ItemContainer occurs in the OnLoadConfigure function since it
        //  depends on data that is initialised by other objects in the OnLoadSetup function
    }

    public override void OnLoadConfigure(SaveData saveData)
    {
        Debug.Log("Loading inventory panel data");

        for (int i = 0; i < slots.Length; i++)
        {
            //Load data for each container slot - the stack size and item type
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

    public void TryAddItemToContainer(Item item)
    {
        //Optional overload for when a bool out type is not needed
        TryAddItemToContainer(item, out bool unused);
    }

    public void TryAddItemToContainer(Item item, out bool itemAdded)
    {
        //Step 1 - loop through all slots to find valid ones

        FindValidInventorySlots(item, out int firstEmptySlot, out int firstStackableSlot);

        //Step 2: If a slot was found, add the item to it in this priority: stackable slot > empty slot

        if (firstStackableSlot == -1 && firstEmptySlot == -1)
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

    private void FindValidInventorySlots(Item item, out int firstEmptySlot, out int firstStackableSlot)
    {
        firstEmptySlot = -1;   //Keeps track of the index of the first empty slot that is found
        firstStackableSlot = -1;   //Keeps track of the index of the first slot where the item can stack that is found

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

    public bool ContainsQuantityOfItem(InventoryItemGroup itemGroup, out List<ContainerSlot> containingSlots)
    {
        int numberOfItemType = 0;
        containingSlots = new List<ContainerSlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemStack.StackSize > 0 && slots[i].ItemStack.StackItemsID == itemGroup.Item.Id)
            {
                numberOfItemType += slots[i].ItemStack.StackSize;
                containingSlots.Add(slots[i]);
            }

            if (numberOfItemType >= itemGroup.Quantity)
            {
                return true;
            }
        }

        return false;
    }
}
