using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{

    RaycastHit raycastHit;

    [SerializeField]
    private Camera playerCamera;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out raycastHit, 4.0f))
            {
                Debug.Log(raycastHit.transform.name);
                if(raycastHit.transform.gameObject.GetComponent<QuestGiver>())
                {
                    raycastHit.transform.gameObject.GetComponent<QuestGiver>().interact();
                }
            }
        }
        
    }
}
