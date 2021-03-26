using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarPanel : UIPanel, IPersistentObject
{
    [SerializeField] private List<ContainerSlotUI>  slotsUI;
    [SerializeField] private ItemContainer          itemContainer;

    public event Action<Item> HeldItemChangedEvent;

    private int selectedSlotIndex;

    protected override void Start()
    {
        base.Start();

        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            += OnSave;
        slm.LoadObjectsSetupEvent       += OnLoadSetup;
        slm.LoadObjectsConfigureEvent   += OnLoadConfigure;

        itemContainer.LinkSlotsToUI(slotsUI);

        itemContainer.ContainerStateChangedEvent += UpdateCurrentSlotSelection;

        SelectSlot(0);

        Show();
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
        CheckForPlayerInput();
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

        HeldItemChangedEvent?.Invoke(GetSelectedItem());
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