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
            for (int j = 0; j < m_itemPool[i].weight; j++)
            {
                weightedItemPool.Add(m_itemPool[i].item);
            }
        }

        return weightedItemPool;
    }
}

[System.Serializable]
public struct WeightedItem
{
    public Item     item;
    public int    weight;
}
