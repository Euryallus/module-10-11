using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxe : HeldTool
{
    [Header("Pickaxe")]
    [SerializeField] private SoundClass pickUpAbilitySound;

    MovableObject heldObj = null;

    protected override void HitDestructibleObject(DestructableObject destructible, RaycastHit raycastHit)
    {
        base.HitDestructibleObject(destructible, raycastHit);

        gameObject.GetComponent<Animator>().SetBool("Swing", true);
    }

    public override void StartSecondardAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        //Check that the player cam isn't null, this can occur in certain cases when an alternate camera is being used (e.g. talking to an NPC)
        if(playerCam != null)
        {
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out toolRaycastHit, 4.0f))
            {
                MovableObject moveObj = toolRaycastHit.transform.gameObject.GetComponent<MovableObject>();

                if (moveObj != null && !moveObj.isHeld && heldObj == null)
                {
                    playerStatsScript.DecreaseFoodLevel(secondaryAbilityHunger);

                    moveObj.PickUp(GameObject.FindGameObjectWithTag("PlayerHand").transform);

                    heldObj = moveObj;

                    if(pickUpAbilitySound != null)
                    {
                        AudioManager.Instance.PlaySoundEffect2D(pickUpAbilitySound);
                    }
                }
            }
        }

        base.StartSecondardAbility();
    }

    public override void EndSecondaryAbility()
    {
        if(heldObj != null)
        {
            heldObj.DropObject(transform.forward);
            heldObj = null;
        }

        base.EndSecondaryAbility();
    }

    public void StopSwing()
    {
        gameObject.GetComponent<Animator>().SetBool("Swing", false);
    }
}
