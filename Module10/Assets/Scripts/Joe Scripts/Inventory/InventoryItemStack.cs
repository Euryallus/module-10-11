using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemStack
{
    public string StackItemsID  { get { return m_stackItemsId; } }
    public int StackSize        { get { return m_stackSize; } }

    private string              m_stackItemsId;
    private int                 m_stackSize;

    public InventoryItemStack()
    {
        m_stackItemsId = "";
        m_stackSize = 0;
    }

    private bool CanAddItemToStack(string itemId)
    {
        if(m_stackSize > 0)
        {
            //Stack is not empty
            if (m_stackItemsId == itemId)
            {
                InventoryItem item = ItemManager.Instance.GetItemWithID(itemId);

                //This stack already contains some of item being added - check the max stack size is not already reached
                if ( m_stackSize < item.GetStackSize())
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

    public bool TryAddItemToStack(string itemId)
    {
        if (CanAddItemToStack(itemId))
        {
            m_stackSize++;

            m_stackItemsId = itemId;

            return true;
        }

        return false;
    }
}