using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryItemStack ItemStack { get { return m_itemStack; } private set { m_itemStack = value; } }

    private InventoryItemStack m_itemStack;

    public void GetItemStack()
    {

    }
}
