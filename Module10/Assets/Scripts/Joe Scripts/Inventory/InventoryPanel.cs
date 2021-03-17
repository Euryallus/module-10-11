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
                m_slots[i].ItemStack.AddItemToStack(itemId, false);
            }

            m_slots[i].UpdateUI();
        }

        UpdateTotalInventoryWeight();
    }

    public void TryAddItemToInventory(InventoryItem item)
    {
        //Step 1 - loop through all slots to find valid ones

        FindValidInventorySlots(item, out int firstEmptySlot, out int firstStackableSlot);

        //Step 2: If a slot was found, add the item to it in this priority: stackable slot > empty slot

        if(firstStackableSlot == -1 && firstEmptySlot == -1)
        {
            //No empty or stackable slots, meaning the inventory is full - warn the player
            Debug.LogWarning("INVENTORY FULL!");
        }
        else
        {
            int chosenSlotIndex;

            if (firstStackableSlot != -1)
            {
                //Stackable slot was found, set it as the chosen slot
                chosenSlotIndex = firstStackableSlot;
            }
            else
            {
                //No stackable slots but an empty slot was found, set it as the chosen slot
                chosenSlotIndex = firstEmptySlot;
            }

            //Add the item to the chosen slot
            m_slots[chosenSlotIndex].ItemStack.AddItemToStack(item.GetID());

            //Update slot UI to show new item
            m_slots[chosenSlotIndex].UpdateUI();

            //Calculate and display new inventory weight
            UpdateTotalInventoryWeight();
        }
    }

    private void FindValidInventorySlots( InventoryItem item, out int firstEmptySlot, out int firstStackableSlot)
    {
        firstEmptySlot      = -1;   //Keeps track of the index of the first empty slot that is found
        firstStackableSlot  = -1;   //Keeps track of the index of the first slot where the item can stack that is found

        for (int i = 0; i < m_slots.Length; i++)
        {
            //Check if the current stack can take the item
            if (m_slots[i].ItemStack.CanAddItemToStack(item.GetID()))
            {
                if (m_slots[i].ItemStack.StackSize == 0 && firstEmptySlot == -1)
                {
                    //The first empty slot was found
                    firstEmptySlot = i;
                }
                else if (m_slots[i].ItemStack.StackSize > 0 && firstStackableSlot == -1)
                {
                    //The first stackable slot was found - no more searching is needed as stackable slots take priority
                    firstStackableSlot = i;
                    return;
                }
            }
        }
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
