using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootChest : Chest, IPersistentObject
{
    [Header("Loot Chest Properties")]
    [SerializeField] private LootTable lootTable;

    private bool lootGenerated;

    protected override void Start()
    {
        base.Start();

        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    public override void Interact()
    {
        base.Interact();

        itemContainer.LinkSlotsToUI(chestPanel.slotsUI);

        if (!lootGenerated)
        {
            GenerateLoot();
        }
        else
        {
            UpdateChestUI();
        }

        chestPanel.Show(false);

        playerInventoryPanel.Show(InventoryShowMode.InventoryOnly, 150.0f);
    }

    private void GenerateLoot()
    {
        int numItemsToSpawn = Random.Range(lootTable.MinItems, lootTable.MaxItems + 1);

        Debug.Log("Generating " + numItemsToSpawn + " loot items for chest with container " + itemContainer.ContainerId);

        int itemsSpawned = 0;

        //Step 1: Add minimum quantity of each item
        for (int i = 0; i < lootTable.ItemPool.Count; i++)
        {
            WeightedItem weightedItem = lootTable.ItemPool[i];
            for (int j = 0; j < weightedItem.MinimumQuantity; j++)
            {
                itemContainer.TryAddItemToContainer(weightedItem.Item);
                itemsSpawned++;
            }
        }

        //Step 2: Fill remaining spaces with random weighted loot
        List<Item> weightedItemPool = lootTable.GetWeightedItemPool();

        while (itemsSpawned < numItemsToSpawn)
        {
            Item itemToAdd = weightedItemPool[Random.Range(0, weightedItemPool.Count)];
            itemContainer.TryAddItemToContainer(itemToAdd);
            itemsSpawned++;
        }

        lootGenerated = true;
    }

    public void OnSave(SaveData saveData)
    {
        saveData.AddData(itemContainer.ContainerId + "_lootGenerated", lootGenerated);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        lootGenerated = saveData.GetData<bool>(itemContainer.ContainerId + "_lootGenerated");
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }
}
