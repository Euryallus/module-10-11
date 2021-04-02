using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContainerSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image            ItemImage       { get { return itemImage; } }
    public GameObject       ItemCountPanel  { get { return itemCountPanel; } }
    public TextMeshProUGUI  ItemCountText   { get { return itemCountText; } }

    [SerializeField] private UIPanel                parentPanel;
    [SerializeField] private Image                  itemImage;
    [SerializeField] private Image                  coverImage;
    [SerializeField] private GameObject             itemCountPanel;
    [SerializeField] private TextMeshProUGUI        itemCountText;
    [SerializeField] private UnityEngine.UI.Outline outline;
    [SerializeField] private Color                  standardOutlineColour;
    [SerializeField] private Color                  selectedOutlineColour;
    [SerializeField] private bool                   clickToAddItems     = true;
    [SerializeField] private bool                   clickToRemoveItems  = true;

    public ContainerSlot Slot { get { return slot; } }

    protected   ContainerSlot   slot;           //The container slot this UI element is linked to
    private     HandSlotUI      handSlotUI;

    private void Start()
    {
        itemImage.gameObject.SetActive(false);
        itemCountPanel.SetActive(false);
        itemCountText.gameObject.SetActive(false);

        handSlotUI = GameObject.FindGameObjectWithTag("HandSlot").GetComponent<HandSlotUI>();
    }

    public void LinkToContainerSlot(ContainerSlot slot)
    {
        this.slot = slot;
        slot.SlotUI = this;
    }

    public void UpdateUI()
    {
        if(slot != null)
        {
            int stackSize = slot.ItemStack.StackSize;

            if (stackSize > 0 && !string.IsNullOrEmpty(slot.ItemStack.StackItemsID))
            {
                Item item = ItemManager.Instance.GetItemWithID(slot.ItemStack.StackItemsID);

                if (item != null)
                {
                    ItemImage.sprite = item.Sprite;
                }
            }

            ItemCountText.text = stackSize.ToString();

            ItemImage.gameObject.SetActive(stackSize > 0);
            ItemCountPanel.SetActive(stackSize > 1);
            ItemCountText.gameObject.SetActive(stackSize > 1);
        }
        else
        {
            ErrorNotLinked();
        }
    }

    public void SetCoverFillAmount(float value)
    {
        if(value == 0.0f)
        {
            coverImage.gameObject.SetActive(false);
        }
        else
        {
            coverImage.gameObject.SetActive(true);
            coverImage.fillAmount = value;
        }
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            outline.effectDistance = new Vector2(2f, 2f);
            outline.effectColor = selectedOutlineColour;
        }
        else
        {
            outline.effectDistance = Vector2.one;
            outline.effectColor = standardOutlineColour;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (parentPanel.Showing)
        {
            if (slot != null)
            {
                if (slot.ItemStack.StackSize > 0)
                {
                    slot.ParentContainer.ItemInfoPopup.ShowPopup(slot.ItemStack.StackItemsID);
                }
            }
            else
            {
                ErrorNotLinked();
            }
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (slot != null)
        {
            slot.ParentContainer.ItemInfoPopup.HidePopup();
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (parentPanel.Showing)
        {
            if (slot != null)
            {
                //Snap the hand slot position to mouse position
                handSlotUI.transform.position = Input.mousePosition;

                bool rightClick = (eventData.button == PointerEventData.InputButton.Right);

                if (clickToRemoveItems && slot.ItemStack.StackSize > 0)
                {
                    slot.MoveItemsToOtherSlot(handSlotUI.Slot, rightClick);
                }
                else if (clickToAddItems && handSlotUI.Slot.ItemStack.StackSize > 0)
                {
                    handSlotUI.Slot.MoveItemsToOtherSlot(slot, rightClick);
                }
            }
            else
            {
                ErrorNotLinked();
            }
        }
    }

    private void ErrorNotLinked()
    {
        Debug.LogError("Slot UI not linked to a ContainerSlot");
    }
}
