using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Type", menuName = "Shop/Shop Type")]
public class ShopType : ScriptableObject
{
    public string           UIName          { get { return m_uiName; } }
    public ShopCategory[]   Categories      { get { return m_categories; } }

    [Header("Shop Properties")]

    [SerializeField] [Tooltip("Name of this shop type to be displayed in the UI")]
    private string          m_uiName;

    [SerializeField] [Tooltip("Each category should contain items that will be grouped together in the shop's interface")]
    private ShopCategory[]  m_categories;
}

[System.Serializable]
public class ShopCategory
{
    public string       UIName          { get { return m_uiName; } }
    public Item         CurrencyItem    { get { return m_currencyItem; } }
    public ShopItem[]   SoldItems       { get { return m_soldItems; } }

    [SerializeField] [Tooltip("Name of this category/tab to be displayed in the UI")]
    private string      m_uiName;

    [SerializeField] [Tooltip("The item required as currency to purchase items in this category")]
    private Item        m_currencyItem;

    [SerializeField] [Tooltip("All items that can be purchased in this category")]
    private ShopItem[]  m_soldItems;
}

[System.Serializable]
public class ShopItem
{
    public Item Item    { get { return m_item; } }
    public int  Price   { get { return m_price; } }

    [SerializeField] [Tooltip("The item to be sold")]
    private Item m_item;

    [SerializeField] [Tooltip("Quantity of the set currency item required to purchase this item")]
    private int  m_price;
}