using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : PersistentObject
{
    //Set in inspector
    [SerializeField]
    private InventorySlot[] m_slots;

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

            m_slots[i].UpdateUI(itemId);
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
                m_slots[i].UpdateUI(item.GetID());

                return;
            }
        }

        //If we get here, the item could not be added to any slot. Warn the player that their inventory is full.
        Debug.LogWarning("INVENTORY FULL!");
    }
}
