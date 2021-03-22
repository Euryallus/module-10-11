using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour, IPersistentObject
{
    public static ItemManager Instance;

    [SerializeField] private Item[]    items;
    public CraftingRecipe[]                     CraftingRecipes;

    private Dictionary<string, Item>   itemsDict           = new Dictionary<string, Item>();
    private Dictionary<string, Item>   customItemsDict     = new Dictionary<string, Item>();
    private Dictionary<string, CraftingRecipe>  craftingRecipesDict = new Dictionary<string, CraftingRecipe>();

    private int customItemUniqueId;

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

        SetupDictionaries();
    }

    private void Start()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent           += OnSave;
        slm.LoadObjectsSetupEvent      += OnLoadSetup;
        slm.LoadObjectsConfigureEvent  += OnLoadConfigure;
    }

    private void OnDestroy()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            -= OnSave;
        slm.LoadObjectsSetupEvent       -= OnLoadSetup;
        slm.LoadObjectsConfigureEvent   -= OnLoadConfigure;
    }

    public void OnSave(SaveData saveData)
    {
        Debug.Log("Saving custom inventory items");

        saveData.AddData("customItemUniqueId", customItemUniqueId);

        saveData.AddData("customItemCount", customItemsDict.Count);

        for (int i = 0; i < customItemsDict.Count; i++)
        {
            Item itemToSave = customItemsDict.ElementAt(i).Value;
            saveData.AddData("customItem" + i, new CustomItemSaveData()
            {
                id            = itemToSave.Id,
                baseItemId    = itemToSave.BaseItemId,
                uiName        = itemToSave.UIName
            } );
        }
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading custom inventory items");

        customItemUniqueId = saveData.GetData<int>("customItemUniqueId");

        int customItemCount = saveData.GetData<int>("customItemCount");

        for (int i = 0; i < customItemCount; i++)
        {
            CustomItemSaveData itemData = saveData.GetData<CustomItemSaveData>("customItem" + i);
            AddCustomItem(itemData.id, itemData.baseItemId);
            SetCustomItemData(itemData.id, itemData.uiName);
        }
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    private void SetupDictionaries()
    {
        //Add all items to a dictionary indexed by their ids
        for (int i = 0; i < items.Length; i++)
        {
            itemsDict.Add(items[i].Id, items[i]);
        }

        //Add all crafting recipes to a dictionary indexed by the resulting item ids
        for (int i = 0; i < CraftingRecipes.Length; i++)
        {
            craftingRecipesDict.Add(CraftingRecipes[i].ResultItem.Item.Id, CraftingRecipes[i]);
        }
    }

    public Item GetItemWithID(string id)
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

    public void AddCustomItem(string id, string baseItemId)
    {
        //Create a duplicate of the base item before editing certain values

        Item baseItem = GetItemWithID(baseItemId);

        if(baseItem!= null)
        {
            Item customItem = Instantiate(GetItemWithID(baseItemId));

            customItem.Id = id;

            customItem.CustomItem = true;

            customItem.BaseItemId = baseItemId;

            if (!customItemsDict.ContainsKey(customItem.Id))
            {
                customItemsDict.Add(customItem.Id, customItem);

                Debug.Log("Added custom item with id: " + customItem.Id);
            }
            else
            {
                Debug.LogWarning("Trying to create custom item with id that already exists: " + customItem.Id);
            }
        }
        else
        {
            Debug.LogError("Trying to create custom item with invalid base item id: " + baseItemId);
        }
    }

    public void SetCustomItemData(string id, string customUIName)
    {
        if (customItemsDict.ContainsKey(id))
        {
            customItemsDict[id].UIName = customUIName;
        }
        else
        {
            Debug.LogError("Trying to set data on custom item with invalid id: " + id);
        }
    }

    public void RemoveCustomItem(string id)
    {
        if (customItemsDict.ContainsKey(id))
        {
            customItemsDict.Remove(id);

            Debug.Log("Removed custom item with id: " + id);
        }
    }

    public string GetUniqueCustomItemId()
    {
        string id = "customItem" + customItemUniqueId;

        return id;
    }

    public void IncrementUniqueCustomItemId()
    {
        customItemUniqueId++;
    }
}

[System.Serializable]
public struct CustomItemSaveData
{
    public string id;
    public string baseItemId;
    public string uiName;
}