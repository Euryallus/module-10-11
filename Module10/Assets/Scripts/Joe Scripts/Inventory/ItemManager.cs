using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : PersistentObject
{
    public static ItemManager Instance;

    [SerializeField] private InventoryItem[] items;

    private Dictionary<string, InventoryItem> itemsDict = new Dictionary<string, InventoryItem>();

    private Dictionary<string, InventoryItem> customItemsDict = new Dictionary<string, InventoryItem>();

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

    public override void OnSave(SaveData saveData)
    {
        Debug.Log("Saving custom inventory items");

        saveData.AddData("customItemCount", customItemsDict.Count);

        //for (int i = 0; i < customItemsDict.Count; i++)
        //{

        //}
    }

    public override void OnLoad(SaveData saveData)
    {
        Debug.Log("Loading custom inventory items");
    }

    private void SetupItemsDict()
    {
        for (int i = 0; i < items.Length; i++)
        {
            itemsDict.Add(items[i].Id, items[i]);
        }
    }

    public InventoryItem GetItemWithID(string id)
    {
        if (itemsDict.ContainsKey(id))
        {
            return itemsDict[id];
        }
        else if (customItemsDict.ContainsKey(id))
        {
            return customItemsDict[id];
        }
        else
        {
            Debug.LogError("Trying to get item with invalid id: " + id);
            return null;
        }
    }

    public void AddCustomItem(string id, string baseItemId, string customUIName)
    {
        InventoryItem baseItem = GetItemWithID(baseItemId);

        InventoryItem customItem = Instantiate(baseItem);

        customItem.UIName = customUIName;

        customItemsDict.Add(id, customItem);
    }
}