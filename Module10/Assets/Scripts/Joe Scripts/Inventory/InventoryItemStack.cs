using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemStack
{
    public string   StackItemsID    { get { return m_stackItemsId; } }
    public int      StackSize       { get { return m_stackSize; } }
    public float    StackWeight     { get { return m_stackWeight; } }

    private string  m_stackItemsId;
    private int     m_stackSize;
    private int     m_maxStackSize;
    private float   m_stackWeight;

    private InventorySlot slot;

    public InventoryItemStack(InventorySlot slot, int maxStackSize)
    {
        this.slot = slot;

        m_stackItemsId  = "";
        m_stackSize     = 0;
        m_maxStackSize  = maxStackSize;
    }

    public bool CanAddItemToStack(string itemId)
    {
        if(m_stackSize < m_maxStackSize || m_maxStackSize == 0)
        {
            if (m_stackSize > 0)
            {
                //Stack is not empty
                if (m_stackItemsId == itemId)
                {
                    InventoryItem item = ItemManager.Instance.GetItemWithID(itemId);

                    //This stack already contains some of item being added - check the max stack size is not already reached
                    if (m_stackSize < item.StackSize)
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
                    //This stack contains a different item type - item cannot be added
                    return false;
                }
            }
            else
            {
                //This stack is empty - item can be added
                return true;
            }
        }
        else
        {
            //This stack has reached its maximum allowed size - item cannot be added
            return false;
        }
    }

    public bool AddItemToStack(string itemId, bool checkIfValid = true)
    {
        if ( !checkIfValid || (checkIfValid && CanAddItemToStack(itemId)) )
        {
            InventoryItem item = ItemManager.Instance.GetItemWithID(itemId);

            if(item != null)
            {
                m_stackSize++;

                m_stackWeight += item.Weight;

                m_stackItemsId = itemId;

                slot.ParentPanel.InventoryStateChanged();

                return true;
            }

            return false;
        }

        return false;
    }

    public bool TryRemoveItemFromStack()
    {
        if(m_stackSize > 0)
        {
            InventoryItem item = ItemManager.Instance.GetItemWithID(m_stackItemsId);

            m_stackSize--;

            m_stackWeight -= item.Weight;

            slot.ParentPanel.InventoryStateChanged();

            return true;
        }

        return false;
    }
}