using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    public int                  MinItems    { get { return m_minItems; } }
    public int                  MaxItems    { get { return m_maxItems; } }
    public List<WeightedItem>   ItemPool    { get { return m_itemPool; } }

    [Space]
    [Header("Loot Table")]
    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("The minumum number of items that can be spawned from this loot table")]
    private int                  m_minItems;

    [SerializeField] [Tooltip("The maximum number of items that can be spawned from this loot table")]
    private int                  m_maxItems;

    [Space]
    [SerializeField] [Tooltip("The items that can be spawned from this loot table, with weights that determine the likelihood of each item spawning")]
    private List<WeightedItem>   m_itemPool;

    public List<Item> GetWeightedItemPool()
    {
        List<Item> weightedItemPool = new List<Item>();

        for (int i = 0; i < m_itemPool.Count; i++)
        {
            for (int j = 0; j < m_itemPool[i].Weight; j++)
            {
                weightedItemPool.Add(m_itemPool[i].Item);
            }
        }

        return weightedItemPool;
    }
}

[System.Serializable]
public struct WeightedItem
{
    [Tooltip("The item that can be added to the loot chest")]
    public Item Item;

    [Tooltip("The likelihood that this item will be added, e.g. if there are 2 items, one with weight 1, and one with weight 3," +
                "the first item will have a 1/4 chance of being added each time, and the second will have a 3/4 chance each time")]
    public int  Weight;

    [Tooltip("At least this many of the item type will be guaranteed to be spawned in the loot chest")]
    public int  MinimumQuantity;
}
