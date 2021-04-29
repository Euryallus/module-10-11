using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public bool PerformingSecondaryAbility { get { return performingSecondaryAbility; } }

    protected   Item            item;
    protected   ContainerSlotUI containerSlot;
    protected   bool            performingSecondaryAbility;

    protected   Transform       playerTransform;
    protected   Transform       playerCameraTransform;

    protected virtual void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public virtual void Setup(Item item, ContainerSlotUI containerSlot)
    {
        this.item           = item;
        this.containerSlot  = containerSlot;
    }

    public virtual void PerformMainAbility()
    {
        //For example, break something
    }

    public virtual void StartSecondardAbility()
    {
        Debug.Log("Starting secondary ability");
        performingSecondaryAbility = true;

        //For example, pick up and start moving an object
    }

    public virtual void EndSecondaryAbility()
    {
        Debug.Log("Ending secondary ability");
        performingSecondaryAbility = false;

        //For example, drop an object
    }

    private void OnDestroy()
    {
        //If the player is performing an ability when this held item is destroyed,
        //  make sure the puzzle ability behaviour is stopped
        if (performingSecondaryAbility)
        {
            EndSecondaryAbility();
        }
    }
}