using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour, IPersistentObject
{
    public static ItemManager Instance;

    [Space] [Header("Item Manager | Please open the prefab to make changes")]

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
            //DontDestroyOnLoad(gameObject);
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
        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
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

            itemData.CustomFloatProperties = new CustomFloatProperty[itemToSave.CustomFloatProperties.Length];

            for (int j = 0; j < itemToSave.CustomFloatProperties.Length; j++)
            {
                itemData.CustomFloatProperties[j] = new CustomFloatProperty()
                {
                    Name            = itemToSave.CustomFloatProperties[j].Name,
                    UIName          = itemToSave.CustomFloatProperties[j].UIName,
                    Value           = itemToSave.CustomFloatProperties[j].Value,
                    UpgradeIncrease = itemToSave.CustomFloatProperties[j].UpgradeIncrease,
                    MinValue        = itemToSave.CustomFloatProperties[j].MinValue,
                    MaxValue        = itemToSave.CustomFloatProperties[j].MaxValue
                };
            }

            itemData.CustomStringProperties = new CustomStringProperty[itemToSave.CustomStringProperties.Length];

            for (int j = 0; j < itemToSave.CustomStringProperties.Length; j++)
            {
                itemData.CustomStringProperties[j] = new CustomStringProperty()
                {
                    Name = itemToSave.CustomStringProperties[j].Name,
                    UIName = itemToSave.CustomStringProperties[j].UIName,
                    Value = itemToSave.CustomStringProperties[j].Value
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

            Item baseItem = GetItemWithID(itemData.BaseItemId);

            //Setup all custom float properties on the new item
            for (int j = 0; j < baseItem.CustomFloatProperties.Length; j++)
            {
                //Get the loaded data for the current custom float property
                var floatPropertyData = itemData.CustomFloatProperties[j];

                //If the loaded data is not null (may occur if the item was created before the custom property was added to the game)
                //  and its property name matches that of the base item (again, old items may have unused properties which should be skipped),
                //  then setup the property data from the loaded values
                if(floatPropertyData != null && floatPropertyData.Name == baseItem.CustomFloatProperties[j].Name)
                {
                    loadedItem.CustomFloatProperties[j] = new CustomFloatProperty()
                    {
                        Name            = floatPropertyData.Name,
                        UIName          = floatPropertyData.UIName,
                        Value           = floatPropertyData.Value,
                        UpgradeIncrease = floatPropertyData.UpgradeIncrease,
                        MinValue        = floatPropertyData.MinValue,
                        MaxValue        = floatPropertyData.MaxValue
                    };
                }
                //If the above condition fails, the item will retain the default property values of its base item
            }

            //Setup all custom string properties on the new item
            for (int j = 0; j < baseItem.CustomStringProperties.Length; j++)
            {
                //Get the loaded data for the current custom string property
                var stringPropertyData = itemData.CustomStringProperties[j];

                //Same process as above, but for string properties
                if (stringPropertyData != null && stringPropertyData.Name == baseItem.CustomStringProperties[j].Name)
                {
                    loadedItem.CustomStringProperties[j] = new CustomStringProperty()
                    {
                        Name = stringPropertyData.Name,
                        UIName = stringPropertyData.UIName,
                        Value = stringPropertyData.Value
                    };
                }
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
                    customItem.CustomFloatProperties[i].Value = baseItem.CustomFloatProperties[i].Value;
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

    public void SetCustomStringItemData(string id, string customPropertyName, string value)
    {
        if (customItemsDict.ContainsKey(id))
        {
            customItemsDict[id].SetCustomStringProperty(customPropertyName, value);
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

    public bool IsItemConsumable(string id)
    {
        Item item = GetItemWithID(id);

        if(item != null && item is ConsumableItem)
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public struct CustomItemSaveData
{
    public string   Id;
    public string   BaseItemId;
    public string   UIName;
    public int      UpgradeLevel;

    public CustomFloatProperty[] CustomFloatProperties;

    public CustomStringProperty[] CustomStringProperties;
}