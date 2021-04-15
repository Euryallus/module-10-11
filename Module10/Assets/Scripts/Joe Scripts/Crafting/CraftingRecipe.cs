using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemGroup> RecipeItems { get { return m_recipeItems; } }
    public ItemGroup       ResultItem  { get { return m_resultItem; } }

    [Space]
    [Header("Recipe Setup")]
    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("Items required to craft the result item")]
    private List<ItemGroup> m_recipeItems;

    [SerializeField] [Tooltip("Items required to craft the result item")]
    private ItemGroup m_resultItem;
}

[System.Serializable]
public class ItemGroup
{
    public ItemGroup(Item item, int quantity)
    {
        Item        = item;
        Quantity    = quantity;
    }

    public Item Item;
    public int  Quantity;
}