using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeldItemManager : MonoBehaviour
{
    private Item heldItem;
    private HotbarPanel hotbarPanel;
    private Transform playerTransform;

    private GameObject      heldGameObject;
    private HeldItem        heldItemScript;
    private ContainerSlotUI heldItemSlot;

    private float mouseHoldTimer;

    private const float mouseHoldThreshold = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        hotbarPanel = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<HotbarPanel>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.Find("Main Camera").transform;

        hotbarPanel.HeldItemChangedEvent += OnHeldItemSelectionChanged;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerInput();
    }

    private void CheckForPlayerInput()
    {
        if(heldItemScript != null)
        {
            if (heldItem != null && heldGameObject != null)
            {
                //Player is holding an item with a HeldItem script attached

                if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    //Player released the left mouse button while their pointer was not over a UI object

                    if (mouseHoldTimer < mouseHoldThreshold)
                    {
                        //If the threshold for a click and release counting as a 'hold' was not reached, perform the held item's main ability
                        heldItemScript.PerformMainAbility();
                    }

                    if (heldItemScript.PerformingSecondaryAbility)
                    {
                        //If the player is currently performing the held item's secondary ability, stop when the mouse button is released
                        heldItemScript.EndSecondaryAbility();
                    }
                }
                else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    //The left mouse button is down and not over a UI object

                    //Increment the timer that keeps track of how long the mouse is held down for
                    UpdateMouseHoldTimer(mouseHoldTimer + Time.unscaledDeltaTime);

                    if (!heldItemScript.PerformingSecondaryAbility && mouseHoldTimer > mouseHoldThreshold)
                    {
                        //If the player is not already performing a secondary ability and the threshold for a mouse 'hold' was reached,
                        //  start the secondary ability behaviour for the held item
                        heldItemScript.StartSecondardAbility();
                    }
                }
                else
                {
                    //Player is not holding/releasing the mouse, default the mouse hold timer to 0
                    UpdateMouseHoldTimer(0.0f);
                }
            }
            else
            {
                //Player is holding nothing/holding an item without an attached GameObject, there is no mouse click/hold
                //  behaviour needed so default the mouse hold timer to 0
                UpdateMouseHoldTimer(0.0f);
            }
        }
    }

    private void UpdateMouseHoldTimer(float value)
    {
        if(mouseHoldTimer != value)
        {
            mouseHoldTimer = value;

            if (heldItemSlot != null && (value == 0.0f || heldItem is ConsumableItem))
            {
                heldItemSlot.SetCoverFillAmount(value / mouseHoldThreshold);
            }
        }
    }

    public void OnHeldItemSelectionChanged(Item item, ContainerSlotUI containerSlot)
    {
        Item oldHeldItem = heldItem;
        heldItem = item;

        UpdateMouseHoldTimer(0.0f);

        heldItemSlot = containerSlot;

        if (heldItem != null)
        {
            if (oldHeldItem == null || heldItem.Id != oldHeldItem.Id)
            {
                Debug.Log("Player is holding " + heldItem.UIName);

                DestroyHeldGameObject();

                if (heldItem.HeldItemGameObject != null)
                {
                    heldGameObject = Instantiate(heldItem.HeldItemGameObject, playerTransform);
                    heldItemScript = heldGameObject.GetComponent<HeldItem>();

                    if(heldItemScript != null)
                    {
                        heldItemScript.Setup(heldItem, containerSlot);
                    }
                }
            }
        }
        else
        {
            DestroyHeldGameObject();

            Debug.Log("Player is holding nothing");
        }
    }

    private void DestroyHeldGameObject()
    {
        if (heldGameObject != null)
        {
            heldItemScript = null;
            Destroy(heldGameObject);
        }
    }
}
