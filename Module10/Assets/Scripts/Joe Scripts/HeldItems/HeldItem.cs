using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public bool PerformingPuzzleAbility { get { return performingPuzzleAbility; } }

    protected   RaycastHit      raycastHit;
    protected   Transform       playerTransform;
    protected   Transform       playerCameraTransform;
    protected   bool            performingPuzzleAbility;
    protected   Item            item;
    protected   ContainerSlotUI containerSlot;

    public void Setup(Item item, ContainerSlotUI containerSlot)
    {
        this.item           = item;
        this.containerSlot  = containerSlot;
    }

    protected virtual void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public virtual void PerformMainAbility()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out raycastHit, 4.0f))
        {
            DestructableObject destructable = raycastHit.transform.gameObject.GetComponent<DestructableObject>();

            if (destructable != null)
            {
                foreach (Item tool in destructable.toolToBreak)
                {
                    string compareId = item.CustomItem ? item.BaseItemId : item.Id;

                    if (tool.Id == compareId)
                    {
                        destructable.TakeHit();
                        break;
                    }
                }
            }
        }
    }

    public virtual void StartSecondardAbility()
    {
        Debug.Log("Starting secondary ability");
        performingPuzzleAbility = true;

        //For example, pick up and start moving an object
    }

    public virtual void EndSecondaryAbility()
    {
        Debug.Log("Ending secondary ability");
        performingPuzzleAbility = false;

        //For example, drop an object
    }

    private void OnDestroy()
    {
        //If the player is performing an ability when this held item is destroyed,
        //  make sure the puzzle ability behaviour is stopped
        if (performingPuzzleAbility)
        {
            EndSecondaryAbility();
        }
    }
}