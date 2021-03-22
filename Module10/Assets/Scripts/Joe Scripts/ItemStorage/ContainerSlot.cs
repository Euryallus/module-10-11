using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ContainerSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    public ItemContainer     ParentContainer;

    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemCountPanel;
    [SerializeField] private TextMeshProUGUI    itemCountText;

    [SerializeField] private int                maxItemCapacity = 0;        //0 means infinite capacity
    [SerializeField] private bool               clickToAddItems = true;
    [SerializeField] private bool               clickToRemoveItems = true;

    #endregion

    public ItemStack ItemStack { get { return itemStack; } private set { itemStack = value; } }

    public event Action ItemsMovedEvent;

    private ItemStack  itemStack;

    private void Awake()
    {
        ItemStack = new ItemStack(this, maxItemCapacity);
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
            Item item = ItemManager.Instance.GetItemWithID(itemStack.StackItemsID);

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
            ParentContainer.ItemInfoPopup.ShowPopup(ItemStack.StackItemsID);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ParentContainer.ItemInfoPopup.HidePopup();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Snap the hand slot position to mouse position
        ParentContainer.HandSlot.transform.position = Input.mousePosition;

        bool rightClick = (eventData.button == PointerEventData.InputButton.Right);

        if (clickToRemoveItems && itemStack.StackSize > 0)
        {
            MoveItemsToOtherSlot(ParentContainer.HandSlot, rightClick);
        }
        else if (clickToAddItems && ParentContainer.HandSlot.ItemStack.StackSize > 0)
        {
            ParentContainer.HandSlot.MoveItemsToOtherSlot(this, rightClick);
        }
    }

    public void MoveItemsToOtherSlot(ContainerSlot otherSlot, bool moveHalf = false)
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

        ItemsMovedEvent?.Invoke();

        otherSlot.ItemsMovedEvent?.Invoke();
    }
}
