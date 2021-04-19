using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarPanel : UIPanel, IPersistentObject
{
    [SerializeField] private List<ContainerSlotUI>  slotsUI;
    [SerializeField] private ItemContainer          itemContainer;
    [SerializeField] private GameObject             itemEatPanel;
    [SerializeField] private CanvasGroup            parentCanvasGroup;

    public event Action<Item, ContainerSlotUI> HeldItemChangedEvent;

    private int         selectedSlotIndex;
    private HandSlotUI  handSlot;
    private bool        showingItemEatPanel;

    private void Awake()
    {
        handSlot = GameObject.FindGameObjectWithTag("HandSlot").GetComponent<HandSlotUI>();
    }

    protected override void Start()
    {
        base.Start();

        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);

        itemEatPanel.SetActive(false);

        itemContainer.LinkSlotsToUI(slotsUI);

        itemContainer.ContainerStateChangedEvent += UpdateCurrentSlotSelection;

        SelectSlot(0);

        ShowHotbarAndStatPanels();
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void Update()
    {
        CheckForPlayerInput();

        if(handSlot.Slot.ItemStack.StackSize > 0 &&
            ItemManager.Instance.IsItemConsumable(handSlot.Slot.ItemStack.StackItemsID))
        {
            if (!showingItemEatPanel)
            {
                itemEatPanel.SetActive(true);
                showingItemEatPanel = true;
            }
        }
        else if(showingItemEatPanel)
        {
            itemEatPanel.SetActive(false);
            showingItemEatPanel = false;
        }
    }

    public void OnSave(SaveData saveData)
    {
        Debug.Log("Saving hotbar panel data");

        saveData.AddData("selectedSlot", selectedSlotIndex);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading hotbar panel data");

        SelectSlot(saveData.GetData<int>("selectedSlot"));
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    public void ShowHotbarAndStatPanels()
    {
        parentCanvasGroup.alpha = 1.0f;
        parentCanvasGroup.blocksRaycasts = true;
    }

    public void HideHotbarAndStatPanels()
    {
        parentCanvasGroup.alpha = 0.0f;
        parentCanvasGroup.blocksRaycasts = false;
    }

    public bool ContainsQuantityOfItem(ItemGroup items)
    {
        return itemContainer.ContainsQuantityOfItem(items, out _);
    }

    public bool RemoveItemFromHotbar(Item item)
    {
        return RemoveItemFromHotbar(item.Id);
    }

    public bool RemoveItemFromHotbar(string itemId)
    {
        return itemContainer.TryRemoveItemFromContainer(itemId);
    }

    private void CheckForPlayerInput()
    {
        //Number keys input
        if (!CustomInputField.AnyFieldSelected)
        {
            for (int i = 1; i <= slotsUI.Count; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    SelectSlot(i - 1);
                    break;
                }
            }
        }

        //Scroll input
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(selectedSlotIndex < (slotsUI.Count - 1))
            {
                SelectSlot(selectedSlotIndex + 1);
            }
            else
            {
                SelectSlot(0);
            }
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(selectedSlotIndex > 0)
            {
                SelectSlot(selectedSlotIndex - 1);
            }
            else
            {
                SelectSlot(slotsUI.Count - 1);
            }
        }
    }

    private void UpdateCurrentSlotSelection()
    {
        SelectSlot(selectedSlotIndex);
    }

    private void SelectSlot(int slotIndex)
    {
        slotsUI[selectedSlotIndex].SetSelected(false);

        selectedSlotIndex = slotIndex;

        slotsUI[selectedSlotIndex].SetSelected(true);

        HeldItemChangedEvent?.Invoke(GetSelectedItem(), slotsUI[selectedSlotIndex]);
    }

    public Item GetSelectedItem()
    {
        ContainerSlot selectedSlot = itemContainer.Slots[selectedSlotIndex];

        if (selectedSlot.ItemStack.StackSize > 0)
        {
            return ItemManager.Instance.GetItemWithID(selectedSlot.ItemStack.StackItemsID);
        }
        else
        {
            //No item in selected slot
            return null;
        }
    }
}