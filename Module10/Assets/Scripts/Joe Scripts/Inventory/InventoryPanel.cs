using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    //Set in inspector
    [SerializeField]
    private InventorySlot[] m_slots;

    public void TryAddItemToInventory(InventoryItem item)
    {
        //Loop through all slots to find a valid one
        for (int i = 0; i < m_slots.Length; i++)
        {
            if(m_slots[i].ItemStack.CanAddItemToStack(item))
            {
                //Item can be added to this slot - add it
                m_slots[i].ItemStack.AddItemToStack(item);

                Debug.Log("Added " + item.GetID() + " to inventory slot " + i);

                //Update slot UI to show new item
                m_slots[i].UpdateUI(item);

                return;
            }
        }

        //If we get here, the item could not be added to any slot. Warn the player that their inventory is full.
        Debug.LogWarning("INVENTORY FULL!");
    }
}
