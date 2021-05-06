using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private List<ItemGroup> collectedOnPickup = new List<ItemGroup>();

    public void PickUp()
    {
        foreach(ItemGroup group in collectedOnPickup)
        {
            for (int i = 0; i < group.Quantity; i++)
            {
                GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>().AddItemToInventory(group.Item);

                
            }
        }

        Destroy(gameObject);
    }
}
