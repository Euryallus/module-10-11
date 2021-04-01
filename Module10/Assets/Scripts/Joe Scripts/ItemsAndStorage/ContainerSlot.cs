using System;

public class ContainerSlot
{
    public ItemContainer ParentContainer { get { return parentContainer; } }
    public ContainerSlotUI SlotUI { get { return slotUI; } set { slotUI = value; } }
    public ItemStack ItemStack { get { return itemStack; } private set { itemStack = value; } }

    public event Action ItemsMovedEvent;

    private ItemStack itemStack;

    private ContainerSlotUI slotUI;
    private ItemContainer   parentContainer;

    public ContainerSlot(int maxItemCapacity, ItemContainer parentContainer)
    {
        ItemStack = new ItemStack(this, maxItemCapacity);
        this.parentContainer = parentContainer;
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

        slotUI.UpdateUI();
        otherSlot.SlotUI.UpdateUI();

        ItemsMovedEvent?.Invoke();

        otherSlot.ItemsMovedEvent?.Invoke();
    }
}
