using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    public InventoryItemStack ItemStack { get { return itemStack; } private set { itemStack = value; } }

    //Set in inspector
    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemCountPanel;
    [SerializeField] private TextMeshProUGUI    itemCountText;
    [SerializeField] private InventoryPanel     parentPanel;
    [SerializeField] private bool               clickableSlot = true;

    private InventoryItemStack itemStack = new InventoryItemStack();

    private void Start()
    {
        //Hide slot UI by default
        itemImage.gameObject    .SetActive(false);
        itemCountPanel          .SetActive(false);
        itemCountText.gameObject.SetActive(false);
    }


    public void UpdateUI()
    {
        if (!string.IsNullOrEmpty(itemStack.StackItemsID))
        {
            InventoryItem item = ItemManager.Instance.GetItemWithID(itemStack.StackItemsID);
            itemImage.sprite = item.GetSprite();
        }

        int stackSize = itemStack.StackSize;

        itemCountText.text = stackSize.ToString();

        itemImage.gameObject    .SetActive(stackSize > 0);
        itemCountPanel          .SetActive(stackSize > 1);
        itemCountText.gameObject.SetActive(stackSize > 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickableSlot)
        {
            //Snap the hand slot position to mouse position
            parentPanel.HandSlot.transform.position = Input.mousePosition;

            bool rightClick = (eventData.button == PointerEventData.InputButton.Right);

            if (itemStack.StackSize > 0)
            {
                MoveItemsToOtherSlot(parentPanel.HandSlot, rightClick);
            }
            else if (parentPanel.HandSlot.ItemStack.StackSize > 0)
            {
                parentPanel.HandSlot.MoveItemsToOtherSlot(this, rightClick);
            }
        }
    }

    public void MoveItemsToOtherSlot(InventorySlot otherSlot, bool moveHalf = false)
    {
        int currentStackSize = (moveHalf ? itemStack.StackSize / 2 : itemStack.StackSize);

        for (int i = 0; i < currentStackSize; i++)
        {
            if (otherSlot.ItemStack.TryAddItemToStack(itemStack.StackItemsID))
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
    }
}
