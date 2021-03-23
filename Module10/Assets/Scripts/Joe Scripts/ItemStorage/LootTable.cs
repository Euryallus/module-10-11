using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
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
}

[System.Serializable]
public struct WeightedItem
{
    public Item     item;
    public float    weight;
}
