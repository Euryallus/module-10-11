using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private InventoryItem[] items;

    private readonly Dictionary<string, InventoryItem> itemsDict = new Dictionary<string, InventoryItem>();

    private void Awake()
    {
        //Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SetupItemsDict();
    }

    private void SetupItemsDict()
    {
        for (int i = 0; i < items.Length; i++)
        {
            itemsDict.Add(items[i].GetID(), items[i]);
        }
    }

    public InventoryItem GetItemWithID(string id)
    {
        if (itemsDict.ContainsKey(id))
        {
            return itemsDict[id];
        }
        else
        {
            Debug.LogError("Trying to get item with invalid id: " + id);
            return null;
        }
    }
}
