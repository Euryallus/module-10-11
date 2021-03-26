using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxe : HeldItem
{
    RaycastHit raycastHit;

    MovableObject heldObj = null;
    public override void PerformMainAbility()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out raycastHit, 4.0f))
        {
            DestructableObject destructable = raycastHit.transform.gameObject.GetComponent<DestructableObject>();
            if (destructable != null)
            {
                foreach(Item tool in destructable.toolToBreak)
                {
                    if(tool.Id == item.Id)
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
            MovableObject moveObj = raycastHit.transform.gameObject.GetComponent<MovableObject>();

            if(moveObj != null && !moveObj.isHeld && heldObj == null)
            {
                moveObj.PickUp(GameObject.FindGameObjectWithTag("PlayerHand").transform);

                heldObj = moveObj;
            }
        }

        base.StartPuzzleAbility();
    }

    public override void EndPuzzleAbility()
    {
        if(heldObj != null)
        {
            heldObj.DropObject(transform.forward);
            heldObj = null;
        }

        base.EndPuzzleAbility();
    }
}
