using System.Collections.Generic;
using UnityEngine;

//CraftingRecipe defines the items required to create another item in the crafting menu

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    #region Properties
    //See tooltips below for info

    public List<ItemGroup> RecipeItems { get { return m_recipeItems; } }
    public ItemGroup       ResultItem  { get { return m_resultItem; } }

    #endregion

    [Space]
    [Header("Recipe Setup")]

    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("Items required to craft the result item")]
    private List<ItemGroup> m_recipeItems;

    [SerializeField] [Tooltip("Items required to craft the result item")]
    private ItemGroup       m_resultItem;
}

//ItemGroup defines a collection of items of the same type
//  e.g. if 5 wood are required for a recipe, a group with item: wood and quantity: 5 can be used

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