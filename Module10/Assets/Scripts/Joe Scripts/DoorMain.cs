using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMain : MonoBehaviour, IPersistentObject
{
    [Header("Door")]

    [SerializeField] private string     id;
    [SerializeField] private Item       unlockItem;
    [SerializeField] private float      closeAfterTime = 5.0f;
    [SerializeField] private Animator   animator;

    private bool openIn;
    private bool openOut;
    private bool unlocked;

    private bool inInsideTrigger;
    private bool inOutsideTrigger;

    private float doorOpenTimer;

    private InventoryPanel  playerInventory;
    private HotbarPanel     playerHotbar;

    private void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();
        playerHotbar    = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<HotbarPanel>();

        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("IMPORTANT: Door exists without id. All doors require a *unique* id for saving/loading data.");
        }
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void Update()
    {
        if(openIn || openOut)
        {
            doorOpenTimer += Time.deltaTime;

            //Door has been open for a while and the player isn't standing in the way of it closing
            if(doorOpenTimer >= closeAfterTime &&
                ((openIn && !inInsideTrigger) || (openOut && !inOutsideTrigger)) )
            {
                SetAsClosed();
            }
        }
    }

    public void OnSave(SaveData saveData)
    {
        Debug.Log("Saving data for door: " + id);

        saveData.AddData("unlocked_" + id, unlocked);

        byte openStateToSave = 0;

        if (openIn)
        {
            openStateToSave = 1;
        }
        else if (openOut)
        {
            openStateToSave = 2;
        }

        saveData.AddData("openState_" + id, openStateToSave);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading data for door: " + id);

        unlocked = saveData.GetData<bool>("unlocked_" + id);

        byte openState = saveData.GetData<byte>("openState_" + id);

        if(openState == 1)
        {
            SetAsOpen(true);
        }
        else if(openState == 2)
        {
            SetAsOpen(false);
        }
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    public void Interact()
    {
        if (!openIn && !openOut)
        {
            if (inInsideTrigger)
            {
                SetAsOpen(false);
            }
            else if(inOutsideTrigger)
            {
                SetAsOpen(true);
            }
        }
        else
        {
            SetAsClosed();
        }
    }

    public void TriggerEntered(bool inside)
    {
        inInsideTrigger = inside;
        inOutsideTrigger = !inside;
    }

    public void TriggerExited(bool inside)
    {
        if (inside)
        {
            inInsideTrigger = false;
        }
        else
        {
            inOutsideTrigger = false;
        }
    }

    private void SetAsOpen(bool inwards)
    {
        if(CanOpenDoor())
        {
            doorOpenTimer = 0.0f;

            openIn = inwards;
            openOut = !inwards;

            if (inwards)
            {
                animator.SetBool("OpenIn", true);
            }
            else
            {
                animator.SetBool("OpenOut", true);
            }
        }
    }

    private bool CanOpenDoor()
    {
        if (unlockItem != null && !unlocked)
        {
            ItemGroup requiredItemGroup = new ItemGroup(unlockItem, 1);

            bool itemInInventory = playerInventory.ContainsQuantityOfItem(requiredItemGroup);
            bool itemInHotbar    = playerHotbar.ContainsQuantityOfItem(requiredItemGroup);

            if (itemInInventory || itemInHotbar)
            {
                unlocked = true;

                NotificationManager.Instance.ShowNotification(NotificationTextType.DoorUnlocked, new string[] { unlockItem.UIName });

                if (itemInInventory)
                {
                    playerInventory.RemoveItemFromInventory(unlockItem);
                }
                else
                {
                    playerHotbar.RemoveItemFromHotbar(unlockItem);
                }
            }
            else
            {
                NotificationManager.Instance.ShowNotification(NotificationTextType.ItemRequiredForDoor, new string[] { unlockItem.UIName });
                return false;
            }
        }

        return true;
    }

    private void SetAsClosed()
    {
        openIn = false;
        openOut = false;

        animator.SetBool("OpenIn", false);
        animator.SetBool("OpenOut", false);
    }
}
