using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IPersistentObject
{
    public ItemInfoPopup ItemInfoPopup;

    public string                               ContainerId;        //Unique id used when saving/loading the contents of this container
    [SerializeField] private int                numberOfSlots;
    [SerializeField] private ContainerSlot[]    slots;              //Main inventory grid

    public ContainerSlot[]  Slots           { get { return slots; } }

    public event Action ContainerStateChangedEvent;     //Event that is invoked when the container state changes (i.e. items are added/removed/moved)
    private bool        containerStateChanged;          //Set to true each time an action occurs that changes the item container's state

    private void Awake()
    {
        //Initialise empty container slots
        slots = new ContainerSlot[numberOfSlots];
        for (int i = 0; i < numberOfSlots; i++)
        {
            slots[i] = new ContainerSlot(0, this);
        }
    }

    private void Start()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            += OnSave;
        slm.LoadObjectsSetupEvent       += OnLoadSetup;
        slm.LoadObjectsConfigureEvent   += OnLoadConfigure;
    }

    private void OnDestroy()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            -= OnSave;
        slm.LoadObjectsSetupEvent       -= OnLoadSetup;
        slm.LoadObjectsConfigureEvent   -= OnLoadConfigure;
    }

    private void Update()
    {
        if (containerStateChanged)
        {
            //The container state was changed one or more times this frame
            ContainerStateChangedThisFrame();
            containerStateChanged = false;
        }
    }

    public void OnSave(SaveData saveData)
    {
        Debug.Log("Saving item container data for " + ContainerId);

        for (int i = 0; i < slots.Length; i++)
        {
            //Save data for each container slot
            saveData.AddData(ContainerId + "_slotStackSize" + i, slots[i].ItemStack.StackSize);
            saveData.AddData(ContainerId + "_stackItemsId" + i, slots[i].ItemStack.StackItemsID);
        }
    }

    public void OnLoadSetup(SaveData saveData)
    {
        //Loading for ItemContainer occurs in the OnLoadConfigure function since it
        //  depends on data that is initialised by other objects in the OnLoadSetup function
    }

    public void OnLoadConfigure(SaveData saveData)
    {
        Debug.Log("Loading item container data for " + ContainerId);

        for (int i = 0; i < slots.Length; i++)
        {
            //Load data for each container slot - the stack size and item type
            int stackSize = saveData.GetData<int>(ContainerId + "_slotStackSize" + i);
            string itemId = saveData.GetData<string>(ContainerId + "_stackItemsId" + i);

            //Add items based on the loaded values
            for (int j = 0; j < stackSize; j++)
            {
                slots[i].ItemStack.AddItemToStack(itemId, false);
            }

            //If slots are linked to UI, update it for each one to reflect changes
            if(slots[i].SlotUI != null)
            {
                slots[i].SlotUI.UpdateUI();
            }
        }
    }

    public void LinkSlotsToUI(List<ContainerSlotUI> slotUIList)
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            slotUIList[i].LinkToContainerSlot(slots[i]);
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

    public void TryAddItemToContainer(Item item)
    {
        //Optional overload for when a bool out type is not needed
        TryAddItemToContainer(item, out bool unused);
    }

    public void TryAddItemToContainer(Item item, out bool itemAdded)
    {
        //Step 1 - loop through all slots to find valid ones

        FindValidContainerSlots(item, out int firstEmptySlot, out int firstStackableSlot);

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
            slots[chosenSlotIndex].SlotUI.UpdateUI();

            itemAdded = true;
        }
    }

    private void FindValidContainerSlots(Item item, out int firstEmptySlot, out int firstStackableSlot)
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
