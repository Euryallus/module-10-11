using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//allows object to be destroyed for items
public class DestructableObject : MonoBehaviour
{
    [Header("Items associated with object")]
    // Items used to destroy object & items collected when destroyed
    public ItemGroup[] itemDroppedOnDestroy;
    public Item[] toolToBreak;

    [Header("Object health")]
    protected int hitsToBreak = 3;

    [SerializeField]
    protected int health;

    private void Start()
    {
        // sets health to default
        health = hitsToBreak;
    }

    public virtual void TakeHit() 
    {
        //if(GameObject.FindGameObjectWithTag())

        --health;

        if(health == 0)
        {
            Destroyed();
        }
    }

    public virtual void Destroyed()
    {
        InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();
        foreach(ItemGroup stack in itemDroppedOnDestroy)
        {
            for (int i = 0; i < stack.Quantity; i++)
            {
                inventory.AddItemToInventory(stack.Item);
            }
        }
    }
}
