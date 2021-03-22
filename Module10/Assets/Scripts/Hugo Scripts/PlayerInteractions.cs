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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out raycastHit, 4.0f))
            {
                if(raycastHit.transform.gameObject.GetComponent<NPC>())
                {
                    if (npcManager != null)
                    {
                        NPC hitNPC = raycastHit.transform.gameObject.GetComponent<NPC>();
                        npcManager.InteractWithNPC(hitNPC);
                    }
                }
            }
        }
        
    }
}
