using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxe : HeldItem
{
    RaycastHit raycastHit;
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
                    if(tool.Id == "pickaxe")
                    {
                        destructable.TakeHit();
                    }
                }
            }
        }
        base.PerformMainAbility();
    }
}
