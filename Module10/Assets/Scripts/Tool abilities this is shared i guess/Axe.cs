using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : HeldItem
{

    Freezable frozenObject = null;

    public override void StartSecondardAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out raycastHit, 4.0f))
        {
            Freezable freeze = raycastHit.transform.gameObject.GetComponent<Freezable>();

            if(freeze != null)
            {
                playerStatsScript.DecreaseFoodLevel(secondaryAbilityHunger);

                frozenObject = freeze;

                frozenObject.Freeze();

                AudioManager.Instance.PlaySoundEffect2D(secondaryAbilitySound);
            }
        }

        base.StartSecondardAbility();
    }

    public override void EndSecondaryAbility()
    {
        if(frozenObject != null)
        {
            frozenObject.UnFreeze();
            frozenObject = null;
        }

        base.EndSecondaryAbility();
    }
}
