using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : PersistentObject
{
    public InventorySlot HandSlot { get { return m_handSlot; } }

    //Set in inspector
    [SerializeField] private InventorySlot[]    m_slots;        //Main inventory grid
    [SerializeField] private InventorySlot      m_handSlot;     //Slot used to pick up and move items

    private void Update()
    {
        if (m_handSlot.ItemStack.StackSize > 0)
        {
            //When there are items in the hand slot, lerp its position to the mouse pointer
            m_handSlot.transform.position = Vector3.Lerp(m_handSlot.transform.position, Input.mousePosition, Time.unscaledDeltaTime * 20.0f);
        }
    }

    public override void OnLoad(SaveData saveData)
    {
        Debug.Log("Loading inventory panel data");

        for (int i = 0; i < m_slots.Length; i++)
        {
            //Load data for each inventory slot
            int stackSize = saveData.GetData<int>("slotStackSize" + i);
            string itemId = saveData.GetData<string>("stackItemsId" + i);

            for (int j = 0; j < stackSize; j++)
            {
                m_slots[i].ItemStack.TryAddItemToStack(itemId);
            }

            m_slots[i].UpdateUI();
        }
    }

    public override void OnSave(SaveData saveData)
    {
        Debug.Log("Saving inventory panel data");

        for (int i = 0; i < m_slots.Length; i++)
        {
            //Save data for each inventory slot
            saveData.AddData("slotStackSize" + i, m_slots[i].ItemStack.StackSize);
            saveData.AddData("stackItemsId" + i, m_slots[i].ItemStack.StackItemsID);
        }
    }

    public void TryAddItemToInventory(InventoryItem item)
    {
        //Loop through all slots to find a valid one
        for (int i = 0; i < m_slots.Length; i++)
        {
            //Try adding the item to the stack
            if(m_slots[i].ItemStack.TryAddItemToStack(item.GetID()))
            {
                //Item was added
                Debug.Log("Added " + item.GetID() + " to inventory slot " + i);

                //Update slot UI to show new item
                m_slots[i].UpdateUI();

                return;
            }
        }

        //If we get here, the item could not be added to any slot. Warn the player that their inventory is full.
        Debug.LogWarning("INVENTORY FULL!");
    }
}
