using UnityEngine;

public class InventoryCustomisation : PersistentObject
{
    [SerializeField] private InventorySlot customiseSlot;   //Slot for the item that will be customised NOTE: SHOULD ONLY EVER ALLOW 1 ITEM
    [SerializeField] private InventorySlot currencySlot;    //Slot for the item(s) used as currency when customising the item in the above slot
    [SerializeField] private InventorySlot resultSlot;      //Slot for the resulting customised item

    public override void OnSave(SaveData saveData)
    {
    }

    public override void OnLoad(SaveData saveData)
    {
    }
}
