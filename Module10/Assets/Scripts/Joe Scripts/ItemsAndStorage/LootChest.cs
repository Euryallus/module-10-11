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
        Debug.Log("Generating loot for chest with container " + itemContainer.ContainerId);

        int numItemsToSpawn = Random.Range(lootTable.MinItems, lootTable.MaxItems + 1);

        List<Item> weightedItemPool = lootTable.GetWeightedItemPool();

        for (int i = 0; i < numItemsToSpawn; i++)
        {
            Item itemToAdd = weightedItemPool[Random.Range(0, weightedItemPool.Count)];
            itemContainer.TryAddItemToContainer(itemToAdd);
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
