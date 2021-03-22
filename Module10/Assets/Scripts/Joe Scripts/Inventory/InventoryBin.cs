using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryBin : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ItemContainer itemContainer;

    public void OnPointerDown(PointerEventData eventData)
    {
        int handStackSize = itemContainer.HandSlot.ItemStack.StackSize;

        //If the hand stack contains any items, remove them all when this bin is clicked
        if (handStackSize > 0)
        {
            for (int i = 0; i < handStackSize; i++)
            {
                itemContainer.HandSlot.ItemStack.TryRemoveItemFromStack();
            }

            //Update hand slot UI to show the player they are no longer holding items
            itemContainer.HandSlot.UpdateUI();
        }
    }
}
