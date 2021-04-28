using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : HeldItem
{
    Freezable frozenObject = null;

    public override void PerformMainAbility()
    {
        //gameObject.GetComponent<Animator>().SetBool("Chop", true);
        base.PerformMainAbility();
    }

    public override void StartSecondardAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        //Check that the player cam isn't null, this can occur in certain cases when an alternate camera is being used (e.g. talking to an NPC)
        if (playerCam != null)
        {
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out raycastHit, 4.0f))
            {
                Freezable freeze = raycastHit.transform.gameObject.GetComponent<Freezable>();

                if (freeze != null)
                {
                    playerStatsScript.DecreaseFoodLevel(secondaryAbilityHunger);

                    frozenObject = freeze;

                    frozenObject.Freeze();

                    AudioManager.Instance.PlaySoundEffect2D(secondaryAbilitySound);
                }
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

    public void StopChop()
    {
        gameObject.GetComponent<Animator>().SetBool("Chop", false);
    }
}
