using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : HeldTool
{
    [Header("Axe")]
    [SerializeField] private SoundClass freezeAbilitySound;
    [SerializeField] private SoundClass objectHitSound;

    Freezable frozenObject = null;

    protected override void HitDestructibleObject(DestructableObject destructible, RaycastHit raycastHit)
    {
        base.HitDestructibleObject(destructible, raycastHit);

        if (!(destructible is NewTree))
        {
            //Play a quick chop animation when hitting a destructible, unless it's a tree because they have a custom axe animation
            gameObject.GetComponent<Animator>().SetTrigger("Chop");
            playerCameraShake.ShakeCameraForTime(0.3f, CameraShakeType.ReduceOverTime, 0.03f);

            if (objectHitSound != null)
            {
                AudioManager.Instance.PlaySoundEffect3D(objectHitSound, raycastHit.point);
            }
        }
    }

    public override void StartSecondardAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        //Check that the player cam isn't null, this can occur in certain cases when an alternate camera is being used (e.g. talking to an NPC)
        if (playerCam != null)
        {
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out toolRaycastHit, 4.0f))
            {
                Freezable freeze = toolRaycastHit.transform.gameObject.GetComponent<Freezable>();

                if (freeze != null)
                {
                    playerStatsScript.DecreaseFoodLevel(secondaryAbilityHunger);

                    frozenObject = freeze;

                    frozenObject.Freeze();

                    if(freezeAbilitySound != null)
                    {
                        AudioManager.Instance.PlaySoundEffect2D(freezeAbilitySound);
                    }
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

    //public void StopChop()
    //{
    //    gameObject.GetComponent<Animator>().SetBool("Chop", false);
    //}
}
