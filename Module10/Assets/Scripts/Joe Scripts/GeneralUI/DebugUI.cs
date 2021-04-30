using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField itemSpawnInputField;

    public void ButtonSpawnItem()
    {
        Item itemToSpawn = ItemManager.Instance.GetItemWithID(itemSpawnInputField.text);

        if(itemToSpawn != null)
        {
            InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();

            inventory.AddItemToInventory(itemToSpawn);
        }
    }
}
