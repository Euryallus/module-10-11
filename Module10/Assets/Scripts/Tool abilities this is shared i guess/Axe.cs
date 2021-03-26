using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : HeldItem
{
    RaycastHit raycastHit;

    Freezable frozenObject = null;

    public override void PerformMainAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out raycastHit, 4.0f))
        {
            Debug.Log(raycastHit.transform.name);

            DestructableObject destructable = raycastHit.transform.gameObject.GetComponent<DestructableObject>();
            if (destructable != null)
            {
                foreach (Item tool in destructable.toolToBreak)
                {
                    if (tool.Id == "axe")
                    {
                        destructable.TakeHit();
                    }
                }
            }
        }

        base.PerformMainAbility();
    }

    public override void StartPuzzleAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out raycastHit, 4.0f))
        {
            Freezable freeze = raycastHit.transform.gameObject.GetComponent<Freezable>();

            if(freeze != null)
            {
                frozenObject = freeze;

                frozenObject.Freeze();
            }
        }

        base.StartPuzzleAbility();
    }

    public override void EndPuzzleAbility()
    {
        if(frozenObject != null)
        {
            frozenObject.UnFreeze();
            frozenObject = null;
        }

        base.EndPuzzleAbility();
    }
}
