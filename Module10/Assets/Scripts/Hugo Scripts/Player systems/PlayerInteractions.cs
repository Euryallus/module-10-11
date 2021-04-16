using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    RaycastHit raycastHit;

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private NPCManager npcManager;

    [SerializeField]
    private MapUI mapUI;


    void Update()
    {
        // runs when player interacts with an object
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out raycastHit, 4.0f))
            {
                if(raycastHit.transform.gameObject.GetComponent<NPC>() != null)
                {
                    // if the player tries to interact with an NPC, call the InteractWithNPC() func. using data from the NPC component
                    if (npcManager != null)
                    {
                        NPC hitNPC = raycastHit.transform.gameObject.GetComponent<NPC>();
                        npcManager.InteractWithNPC(hitNPC);
                    }
                }
                //else if(raycastHit.transform.gameObject.GetComponent<DestructableObject>() != null)
                //{
                //    // if player tries to interact with destructable object, check if object is hit
                //    raycastHit.transform.gameObject.GetComponent<DestructableObject>().TakeHit();
                //}

                //if(raycastHit.transform.gameObject.CompareTag("Ladder"))
                //{
                //    gameObject.GetComponent<PlayerMovement>().InteractWithLadder();
                //}
            }
        }
        
    }

}
