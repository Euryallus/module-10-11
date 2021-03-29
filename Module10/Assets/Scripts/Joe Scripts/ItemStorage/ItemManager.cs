using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour, IPersistentObject
{
    public static ItemManager Instance;

    [SerializeField] private Item[]             items;
    public CraftingRecipe[]                     CraftingRecipes;

    private Dictionary<string, Item>            itemsDict           = new Dictionary<string, Item>();
    private Dictionary<string, Item>            customItemsDict     = new Dictionary<string, Item>();
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

            CustomItemSaveData itemData = new CustomItemSaveData()
            {
                Id = itemToSave.Id,
                BaseItemId = itemToSave.BaseItemId,
                UIName = itemToSave.UIName,
            };

            itemData.CustomFloatProperties = new CustomItemProperty<float>[itemToSave.CustomFloatProperties.Length];

            for (int j = 0; j < itemToSave.CustomFloatProperties.Length; j++)
            {
                itemData.CustomFloatProperties[j] = new CustomItemProperty<float>()
                {
                    Name            = itemToSave.CustomFloatProperties[j].Name,
                    UIName          = itemToSave.CustomFloatProperties[j].UIName,
                    Value           = itemToSave.CustomFloatProperties[j].Value,
                    UpgradeIncrease = itemToSave.CustomFloatProperties[j].UpgradeIncrease,
                    MinValue        = itemToSave.CustomFloatProperties[j].MinValue,
                    MaxValue        = itemToSave.CustomFloatProperties[j].MaxValue
                };
            }

            saveData.AddData("customItem" + i, itemData );
        }
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading custom inventory items");

        //Load the unique id to be used when creating a custom item
        customItemUniqueId = saveData.GetData<int>("customItemUniqueId");

        //Load the number of custom items that player had created
        int customItemCount = saveData.GetData<int>("customItemCount");

        //Load all custom items
        for (int i = 0; i < customItemCount; i++)
        {
            //Get the save data for the current custom item
            CustomItemSaveData itemData = saveData.GetData<CustomItemSaveData>("customItem" + i);

            //Add the custom item based on the loaded ids
            Item loadedItem = AddCustomItem(itemData.Id, itemData.BaseItemId, itemData.BaseItemId);

            //Set the UI name of the custom item from the loaded value
            SetCustomGenericItemData(itemData.Id, itemData.UIName);

            //Get all of the default custom float properties from the base item
            var baseItemFloatProperties = GetItemWithID(itemData.BaseItemId).CustomFloatProperties;

            //Setup all custom float properties on the new item
            for (int j = 0; j < baseItemFloatProperties.Length; j++)
            {
                //Get the loaded data for the current custom float property
                var propertyData = itemData.CustomFloatProperties[j];

                //If the loaded data is not null (may occur if the item was created before the custom property was added to the game)
                //  and its property name matches that of the base item (again, old items may have unused properties which should be skipped),
                //  then setup the property data from the loaded values
                if(propertyData != null && propertyData.Name == baseItemFloatProperties[j].Name)
                {
                    loadedItem.CustomFloatProperties[j] = new CustomItemProperty<float>()
                    {
                        Name            = propertyData.Name,
                        UIName          = propertyData.UIName,
                        Value           = propertyData.Value,
                        UpgradeIncrease = propertyData.UpgradeIncrease,
                        MinValue        = propertyData.MinValue,
                        MaxValue        = propertyData.MaxValue
                    };
                }
                //If the above condition fails, the item will retain the default property values of its base item
            }
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

    public Item AddCustomItem(string id, string baseItemId, string originalBaseItemId)
    {
        //Create a duplicate of the base item before editing certain values

        Item baseItem = GetItemWithID(baseItemId);
        Item originalBaseItem = GetItemWithID(originalBaseItemId);

        if(originalBaseItem != null)
        {
            Item customItem = Instantiate(GetItemWithID(originalBaseItemId));

            customItem.Id = id;

            customItem.CustomItem = true;

            customItem.BaseItemId = originalBaseItemId;

            if(baseItem != null)
            {
                //Setup custom float properties from base item
                for (int i = 0; i < baseItem.CustomFloatProperties.Length; i++)
                {
                    customItem.CustomFloatProperties[i] = baseItem.CustomFloatProperties[i];
                }
            }
            else
            {
                Debug.LogError("Trying to create custom item with invalid base item id: " + originalBaseItemId);
            }

            if (!customItemsDict.ContainsKey(customItem.Id))
            {
                customItemsDict.Add(customItem.Id, customItem);

                Debug.Log("Added custom item with id: " + customItem.Id);

                return customItem;
            }
            else
            {
                Debug.LogWarning("Trying to create custom item with id that already exists: " + customItem.Id);
                return null;
            }
        }
        else
        {
            Debug.LogError("Trying to create custom item with invalid original base item id: " + originalBaseItemId);
            return null;
        }
    }

    public Item GetCustomItem(string id)
    {
        if (customItemsDict.ContainsKey(id))
        {
            return customItemsDict[id];
        }
        else
        {
            Debug.LogError("Trying to get custom item with invalid id: " + id);
            return null;
        }
    }

    public void SetCustomFloatItemData(string id, string customPropertyName, float value)
    {
        if (customItemsDict.ContainsKey(id))
        {
            customItemsDict[id].SetCustomFloatProperty(customPropertyName, value);
        }
        else
        {
            Debug.LogError("Trying to set data on custom item with invalid id: " + id);
        }
    }

    public void SetCustomGenericItemData(string id, string customUIName)
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
    public string   Id;
    public string   BaseItemId;
    public string   UIName;
    public int      UpgradeLevel;

    public CustomItemProperty<float>[] CustomFloatProperties;
}