using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "Crafting/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<InventoryItemGroup> RecipeItems { get { return m_recipeItems; } }
    public InventoryItemGroup       ResultItem  { get { return m_resultItem; } }

    [Header("Recipe Setup")]
    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("Items required to craft the result item")]
    private List<InventoryItemGroup> m_recipeItems;

    [SerializeField] [Tooltip("Items required to craft the result item")]
    private InventoryItemGroup m_resultItem;
}

[System.Serializable]
public class InventoryItemGroup
{
    public InventoryItem    Item;
    public int              Quantity;
}