using UnityEngine;
using UnityEngine.EventSystems;

public class EatItemPanel : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        HandSlotUI handSlotUI = GameObject.FindGameObjectWithTag("HandSlot").GetComponent<HandSlotUI>();

        int handStackSize = handSlotUI.Slot.ItemStack.StackSize;

        //If the hand stack contains any items, remove one when this panel is clicked
        if (handStackSize > 0)
        {
            Item itemInHand = ItemManager.Instance.GetItemWithID(handSlotUI.Slot.ItemStack.StackItemsID);

            if(itemInHand is ConsumableItem consumable)
            {
                PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

                if (!playerStats.PlayerIsFull())
                {
                    handSlotUI.Slot.ItemStack.TryRemoveItemFromStack();

                    playerStats.IncreaseFoodLevel(consumable.HungerIncrease);

                    handSlotUI.UpdateUI();
                }
            }
            else
            {
                Debug.LogError("Should never be able to click on the eat item panel with a non-consumable item");
            }
        }
    }
}