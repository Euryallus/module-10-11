using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    //Set in inspector
    [SerializeField]
    private InventorySlot[] m_slots;

    private const int InventoryGridWidth    = 8;
    private const int InventoryGridHeight   = 3;

    private void Start()
    {
    }

    public void TryAddItemToInventory(InventoryItem item)
    {
        //Loop through all slots to find a valid one
        for (int i = 0; i < m_slots.Length; i++)
        {
            if(m_slots[i].ItemStack.CanAddItemToStack(item))
            {
                m_slots[i].ItemStack.AddItemToStack(item);
                break;
            }
        }

        //If we get here, the item could not be added to any slot. Warn the player that their inventory is full.
        Debug.LogWarning("INVENTORY FULL!");
    }
}
