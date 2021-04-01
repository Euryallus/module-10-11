using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item")]
public class Item : ScriptableObject
{
    public string                       Id { get { return m_id; } set { m_id = value; } }
    public string                       UIName { get { return m_uiName; } set { m_uiName = value; } }
    public string                       UIDescription { get { return m_uiDescription; } }
    public int                          StackSize { get { return m_stackSize; } }
    public float                        Weight { get { return m_weight; } }
    public Sprite                       Sprite { get { return m_sprite; } }
    public GameObject                   HeldItemGameObject { get { return m_heldItemGameObject; } }
    public bool                         Customisable { get { return m_customisable; } }
    public string                       CurrencyItemId { get { return m_currencyItemId; } }
    public int                          CurrencyItemQuantity { get { return m_currencyItemQuantity; } }
    public bool                         CustomItem { get { return m_customItem; } set { m_customItem = value; } }
    public string                       BaseItemId { get { return m_baseItemId; } set { m_baseItemId = value; } }
    public CustomItemProperty<float>[]  CustomFloatProperties { get { return m_customFloatProperties; } set { m_customFloatProperties = value; } }

    [Space]
    [Header("Info")]
    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("Unique identifier for this item")]
    private string m_id;

    [SerializeField] [Tooltip("Name to be displayed in the user interface")]
    private string m_uiName;

    [SerializeField] [Tooltip("Short description to be displayed in the user interface. Can be left blank")]
    private string m_uiDescription;

    [SerializeField] [Tooltip("Maximum number of this item that can be stored in a single stack")]
    private int m_stackSize = 1;

    [SerializeField] [Tooltip("The weight of this item - more desirable items should have a greater weight")]
    private float m_weight;

    [SerializeField] [Tooltip("Sprite to be displayed in the UI for this item")]
    private Sprite m_sprite;

    [SerializeField] [Tooltip("The GameObject to be instantiated when the player holds this item, leave blank if no held item should be shown")]
    private GameObject m_heldItemGameObject;

    [Space]
    [Header("Player Customisation")]

    [SerializeField] [Tooltip("Whether or not the player can customise this item type's name/properties")]
    private bool m_customisable;

    [SerializeField] [Tooltip("The id of the item needed to customise this item type. Leave blank if this item is not customisable, or does not require another item to be customised.")]
    private string m_currencyItemId;

    [SerializeField] [Tooltip("The number of the above items required to customise this item type. Leave at 0 if this item is not customisable, or does not require another item to be customised.")]
    private int m_currencyItemQuantity;

    [Space]
    [SerializeField] [Tooltip("Properties with float values that can be upgraded/customised by the player.")]
    private CustomItemProperty<float>[] m_customFloatProperties;

    //Non-editable fields
    private bool    m_customItem;
    private string  m_baseItemId;

    public CustomItemProperty<float> GetCustomFloatPropertyWithName(string propertyName)
    {
        for (int i = 0; i < m_customFloatProperties.Length; i++)
        {
            if(m_customFloatProperties[i].Name == propertyName)
            {
                return m_customFloatProperties[i];
            }
        }

        Debug.LogError("Trying to get invalid custom float property: " + propertyName);
        return default;
    }

    public void SetCustomFloatProperty(string propertyName, float value)
    {
        for (int i = 0; i < m_customFloatProperties.Length; i++)
        {
            if (m_customFloatProperties[i].Name == propertyName)
            {
                m_customFloatProperties[i].Value = value;
                return;
            }
        }

        Debug.LogError("Trying to set invalid custom float property: " + propertyName);
    }

}

[System.Serializable]
public class CustomItemProperty<T>
{
    [Tooltip("The name used to get/set this property in code")]
    public string   Name;

    [Tooltip("The name that will be displayed for this property in the user interface")]
    public string   UIName;

    [Tooltip("The default value of this property")]
    public T        Value;

    [Tooltip("How much the value changed by each time the +/- button is pressed when customising")]
    public T        UpgradeIncrease;

    [Tooltip("The Value cannot go below this no matter how many times the item is customised")]
    public T        MinValue;

    [Tooltip("The Value cannot surpass this no matter how many times the item is customised")]
    public T        MaxValue;
}