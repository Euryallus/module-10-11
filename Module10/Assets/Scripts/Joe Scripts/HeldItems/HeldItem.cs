using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    [Header("Food level reduction when item is used (for tools):")]

    [SerializeField] [Tooltip("How much the player's food level decreases when the item's main ability is used")]
    protected float mainAbilityHunger;

    [SerializeField] [Tooltip("How much the player's food level decreases when the item's secondary ability is used")]
    protected float secondaryAbilityHunger;

    [SerializeField] protected SoundClass primaryAbilitySound;
    [SerializeField] protected SoundClass secondaryAbilitySound;

    public bool PerformingSecondaryAbility { get { return performingSecondaryAbility; } }

    protected   RaycastHit      raycastHit;
    protected   Transform       playerTransform;
    protected   Transform       playerCameraTransform;
    protected   bool            performingSecondaryAbility;
    protected   Item            item;
    protected   ContainerSlotUI containerSlot;
    protected   PlayerStats     playerStatsScript;

    public virtual void Setup(Item item, ContainerSlotUI containerSlot)
    {
        this.item           = item;
        this.containerSlot  = containerSlot;
    }

    protected virtual void Awake()
    {
        playerTransform         = GameObject.FindGameObjectWithTag("Player").transform;
        playerCameraTransform   = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerStatsScript       = playerTransform.GetComponent<PlayerStats>();
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
                        playerStatsScript.DecreaseFoodLevel(mainAbilityHunger);

                        destructable.TakeHit();

                        if(primaryAbilitySound != null)
                        {
                            AudioManager.Instance.PlaySoundEffect2D(primaryAbilitySound);
                        }

                        break;
                    }
                }
            }
        }
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