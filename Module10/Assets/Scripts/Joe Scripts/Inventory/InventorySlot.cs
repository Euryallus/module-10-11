using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemCountPanel;
    [SerializeField] private TextMeshProUGUI    itemCountText;
    [SerializeField] private InventoryPanel     parentPanel;

    [SerializeField] private int                maxItemCapacity = 0;        //0 means infinite capacity
    [SerializeField] private bool               clickToAddItems = true;
    [SerializeField] private bool               clickToRemoveItems = true;

    #endregion

    public InventoryItemStack ItemStack { get { return itemStack; } private set { itemStack = value; } }

    public event Action ItemsMovedEvent;

    private InventoryItemStack itemStack;

    private void Awake()
    {
        ItemStack = new InventoryItemStack(maxItemCapacity);
    }

    private void Start()
    {
        //Hide slot UI by default
        itemImage.gameObject    .SetActive(false);
        itemCountPanel          .SetActive(false);
        itemCountText.gameObject.SetActive(false);
    }


    public void UpdateUI()
    {
        int stackSize = itemStack.StackSize;

        if (stackSize > 0 && !string.IsNullOrEmpty(itemStack.StackItemsID))
        {
            InventoryItem item = ItemManager.Instance.GetItemWithID(itemStack.StackItemsID);

            if(item != null)
            {
                itemImage.sprite = item.Sprite;
            }
        }

        itemCountText.text = stackSize.ToString();

        itemImage.gameObject    .SetActive(stackSize > 0);
        itemCountPanel          .SetActive(stackSize > 1);
        itemCountText.gameObject.SetActive(stackSize > 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(ItemStack.StackSize > 0)
        {
            parentPanel.ItemInfoPopup.ShowPopup(ItemStack.StackItemsID);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        parentPanel.ItemInfoPopup.HidePopup();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Snap the hand slot position to mouse position
        parentPanel.HandSlot.transform.position = Input.mousePosition;

        bool rightClick = (eventData.button == PointerEventData.InputButton.Right);

        if (clickToRemoveItems && itemStack.StackSize > 0)
        {
            MoveItemsToOtherSlot(parentPanel.HandSlot, rightClick);
        }
        else if (clickToAddItems && parentPanel.HandSlot.ItemStack.StackSize > 0)
        {
            parentPanel.HandSlot.MoveItemsToOtherSlot(this, rightClick);
        }
    }

    public void MoveItemsToOtherSlot(InventorySlot otherSlot, bool moveHalf = false)
    {
        int currentStackSize = (moveHalf ? itemStack.StackSize / 2 : itemStack.StackSize);

        for (int i = 0; i < currentStackSize; i++)
        {
            if (otherSlot.ItemStack.AddItemToStack(itemStack.StackItemsID))
            {
                itemStack.TryRemoveItemFromStack();
            }
            else
            {
                break;
            }
        }

        UpdateUI();
        otherSlot.UpdateUI();

        parentPanel.UpdateTotalInventoryWeight();

        ItemsMovedEvent?.Invoke();

        otherSlot.ItemsMovedEvent?.Invoke();
    }
}
