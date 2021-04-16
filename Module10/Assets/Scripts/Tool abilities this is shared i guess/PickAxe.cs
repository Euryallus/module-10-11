using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxe : HeldItem
{
    MovableObject heldObj = null;

    public override void PerformMainAbility()
    {
        gameObject.GetComponent<Animator>().SetBool("Swing", true);
        base.PerformMainAbility();
    }

    public override void StartSecondardAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out raycastHit, 4.0f))
        {
            MovableObject moveObj = raycastHit.transform.gameObject.GetComponent<MovableObject>();

            if(moveObj != null && !moveObj.isHeld && heldObj == null)
            {
                playerStatsScript.DecreaseFoodLevel(secondaryAbilityHunger);

                moveObj.PickUp(GameObject.FindGameObjectWithTag("PlayerHand").transform);

                heldObj = moveObj;

                AudioManager.Instance.PlaySoundEffect2D(secondaryAbilitySound);
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
