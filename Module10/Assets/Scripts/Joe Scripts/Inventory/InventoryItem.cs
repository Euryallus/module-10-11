using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string   Id                      { get { return m_id; } set { m_id = value; } }
    public string   UIName                  { get { return m_uiName; } set { m_uiName = value; } }
    public int      StackSize               { get { return m_stackSize; } }
    public float    Weight                  { get { return m_weight; } }
    public Sprite   Sprite                  { get { return m_sprite; } }
    public bool     Customisable            { get { return m_customisable; } }
    public string   CurrencyItemId          { get { return m_currencyItemId; } }
    public int      CurrencyItemQuantity    { get { return m_currencyItemQuantity; } }  
    public bool     CustomItem              { get { return m_customItem; } set { m_customItem = value; } }         
    public string   BaseItemId              { get { return m_baseItemId; } set { m_baseItemId = value; } }         

    [Header("Info")]
    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("Unique identifier for this item")]
    private string m_id;

    [SerializeField] [Tooltip("Name to be displayed in the user interface")]
    private string m_uiName;

    [SerializeField] [Tooltip("Maximum number of this item that can be stored in a single stack")]
    private int m_stackSize = 1;

    [SerializeField] [Tooltip("The weight of this item - more desirable items should have a greater weight")]
    private float m_weight;

    [SerializeField] [Tooltip("Sprite to be displayed in the UI for this item")]
    private Sprite m_sprite;

    [Header("Player Customisation")]

    [SerializeField] [Tooltip("Whether or not the player can rename this item type")]
    private bool m_customisable;

    [SerializeField] [Tooltip("The id of the item needed to rename this item type. Leave blank if this item is not renamable, or does not require another item to be renamed.")]
    private string m_currencyItemId;

    [SerializeField] [Tooltip("The number of the above items required to rename this item type. Leave at 0 if this item is not renamable, or does not require another item to be renamed.")]
    private int m_currencyItemQuantity;

    //Non-editable fields
    private bool    m_customItem;
    private string  m_baseItemId;

}