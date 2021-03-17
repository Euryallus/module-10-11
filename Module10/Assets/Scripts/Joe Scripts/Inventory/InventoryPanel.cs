using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPanel : PersistentObject
{
    public InventorySlot HandSlot { get { return m_handSlot; } }

    //Fields marked with [SerializeField] are set in the inspector
    [SerializeField] private InventorySlot[]    m_slots;                //Main inventory grid
    [SerializeField] private InventorySlot      m_handSlot;             //Slot used to pick up and move items

    [SerializeField] private TextMeshProUGUI    m_weightText;           //Text displaying how full the inventory is
    [SerializeField] private Slider             m_weightSlider;         //Slider that shows how close the inventory is to holding its max weight
    [SerializeField] private Image              m_sliderFillImage;      //Image used on the slider to show how full the inventory is
    [SerializeField] private Color              m_sliderStandardColour; //Default colour of the slider image
    [SerializeField] private Color              m_sliderFullColour;     //Colour of the slider image when the inventory is full

    [SerializeField] private float              m_maxWeight;            //Maximum amount of weight this inventory can hold

    private float m_totalWeight = 0.0f;   //The current amount of weight of all items in the inventory

    protected override void Start()
    {
        base.Start();

        UpdateTotalInventoryWeight();
    }

    private void Update()
    {
        if (m_handSlot.ItemStack.StackSize > 0)
        {
            //When there are items in the hand slot, lerp its position to the mouse pointer
            m_handSlot.transform.position = Vector3.Lerp(m_handSlot.transform.position, Input.mousePosition, Time.unscaledDeltaTime * 20.0f);
        }
    }

    public override void OnSave(SaveData saveData)
    {
        Debug.Log("Saving inventory panel data");

        for (int i = 0; i < m_slots.Length; i++)
        {
            //Save data for each inventory slot
            saveData.AddData("slotStackSize" + i, m_slots[i].ItemStack.StackSize);
            saveData.AddData("stackItemsId" + i, m_slots[i].ItemStack.StackItemsID);
        }
    }

    public override void OnLoad(SaveData saveData)
    {
        Debug.Log("Loading inventory panel data");

        for (int i = 0; i < m_slots.Length; i++)
        {
            //Load data for each inventory slot
            int stackSize = saveData.GetData<int>("slotStackSize" + i);
            string itemId = saveData.GetData<string>("stackItemsId" + i);

            for (int j = 0; j < stackSize; j++)
            {
                m_slots[i].ItemStack.TryAddItemToStack(itemId);
            }

            m_slots[i].UpdateUI();
        }

        UpdateTotalInventoryWeight();
    }

    public void TryAddItemToInventory(InventoryItem item)
    {
        //Loop through all slots to find a valid one
        for (int i = 0; i < m_slots.Length; i++)
        {
            //Try adding the item to the stack
            if(m_slots[i].ItemStack.TryAddItemToStack(item.GetID()))
            {
                //Item was added
                Debug.Log("Added " + item.GetID() + " to inventory slot " + i);

                //Update slot UI to show new item
                m_slots[i].UpdateUI();

                UpdateTotalInventoryWeight();

                return;
            }
        }

        //If we get here, the item could not be added to any slot. Warn the player that their inventory is full.
        Debug.LogWarning("INVENTORY FULL!");
    }

    public void UpdateTotalInventoryWeight()
    {
        float weight = 0.0f;

        for (int i = 0; i < m_slots.Length; i++)
        {
            weight += m_slots[i].ItemStack.StackWeight;
        }

        m_totalWeight = weight;

        Debug.Log("Total inventory weight is " + m_totalWeight);

        float weightVal = m_totalWeight / m_maxWeight;

        m_weightSlider.value = Mathf.Clamp(weightVal, 0.0f, 1.0f);

        m_weightText.text = "Weight Limit (" + Mathf.RoundToInt(weightVal * 100.0f) + "%)";

        if(m_totalWeight >= m_maxWeight)
        {
            m_sliderFillImage.color = m_sliderFullColour;
            Debug.LogWarning("MAX INVENTORY WEIGHT REACHED!");
        }
        else
        {
            m_sliderFillImage.color = m_sliderStandardColour;
        }
    }
}
