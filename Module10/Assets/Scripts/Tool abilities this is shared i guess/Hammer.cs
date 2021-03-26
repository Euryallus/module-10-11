using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : HeldItem
{
    RaycastHit      raycastHit;
    Transform       playerCameraTransform;
    PlayerMovement  playerMovementScript;

    private void Start()
    {
        playerCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public override void PerformMainAbility()
    {
        base.PerformMainAbility();

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out raycastHit, 4.0f))
        {
            DestructableObject destructable = raycastHit.transform.gameObject.GetComponent<DestructableObject>();

            if (destructable != null)
            {
                foreach (Item tool in destructable.toolToBreak)
                {
                    if (tool.Id == item.BaseItemId)
                    {
                        destructable.TakeHit();
                        break;
                    }
                }
            }
        }
    }

    public override void StartPuzzleAbility()
    {
        base.StartPuzzleAbility();

        playerMovementScript.SetJumpVelocity(8.0f);   //Change to item.LaunchVelocity or similar
        Debug.Log("WTF");
    }
}
