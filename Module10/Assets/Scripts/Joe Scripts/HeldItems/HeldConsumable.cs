using UnityEngine;

public class HeldConsumable : HeldItem
{
    [Header("Consumable")]
    [SerializeField] private SoundClass eatSound;

    public override void StartSecondardAbility()
    {
        base.StartSecondardAbility();

        if(item is ConsumableItem consumable)
        {
            //Get a reference to the player's PlayerStats script
            PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            if (!playerStats.PlayerIsFull())
            {
                //Increase the player's food level depending on the item's HungerIncrease value
                playerStats.IncreaseFoodLevel(consumable.HungerIncrease);

                //Remove the item that was consumed from its stack
                containerSlot.Slot.ItemStack.TryRemoveItemFromStack();

                //Update the slot UI to show that an item was removed
                containerSlot.UpdateUI();

                if(eatSound != null)
                {
                    AudioManager.Instance.PlaySoundEffect2D(eatSound);
                }
            }
            else
            {
                NotificationManager.Instance.AddNotificationToQueue(NotificationMessageType.PlayerTooFull);
            }
        }
        else
        {
            Debug.LogError("HeldConsumable script should never be attached to a non-consulable item");
        }
    }
}
