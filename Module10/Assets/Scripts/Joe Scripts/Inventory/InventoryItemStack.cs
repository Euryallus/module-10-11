using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemStack
{
    private string m_stackItemsId;
    private List<InventoryItem> m_items;

    public InventoryItemStack(InventoryItem[] items)
    {
        m_stackItemsId = "";
        m_items = new List<InventoryItem>();

        for (int i = 0; i < items.Length; i++)
        {
            m_items.Add(items[i]);
        }
    }

    public bool CanAddItemToStack(InventoryItem item)
    {
        if(m_items.Count > 0)
        {
            //Stack is not empty
            if (m_stackItemsId == item.GetID())
            {
                //This stack already contains some of item being added - check the max stack size is not already reached
                if( m_items.Count < item.GetStackSize())
                {
                    //Max stack size not reached - item can be added
                    return true;
                }
                else
                {
                    //Max stack size reached - item cannot be added
                    return false;
                }
            }
            else
            {
                //This stack contains a different item type - can't add item here
                return false;
            }
        }
        else
        {
            //This stack is empty - item can be added
            return true;
        }
    }

    public void AddItemToStack(InventoryItem item)
    {
        m_items.Add(item);

        m_stackItemsId = item.GetID();
    }
}
