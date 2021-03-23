using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField]
    private QuestManager qmanager;

    private DialogueUI UI;

    private NPC interactingWith = null;
    private PlayerMovement playerMove;
    [SerializeField]
    private GameObject playerCamera;

    private enum focusCameraState
    { 
        normal,
        moving,
        focused
    }

    private focusCameraState focusCameraCurrentState = focusCameraState.normal;
    private Transform targetCameraTransform;

    [SerializeField]
    private GameObject focusCamera;
    [SerializeField]
    private float cameraLerpSpeed;


    private void Start()
    {
        UI = gameObject.GetComponent<DialogueUI>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        UI.HideDialogue();

        focusCamera.SetActive(false);
    }

    private void Update()
    {
        if(focusCameraCurrentState == focusCameraState.moving)
        {
            focusCamera.transform.position = Vector3.Lerp(focusCamera.transform.position, targetCameraTransform.position, cameraLerpSpeed * Time.deltaTime);
            focusCamera.transform.rotation = Quaternion.Lerp(focusCamera.transform.rotation, targetCameraTransform.rotation, cameraLerpSpeed * Time.deltaTime);

            if(Vector3.Distance( focusCamera.transform.position, targetCameraTransform.position) < 0.01f)
            {
                focusCamera.transform.position = targetCameraTransform.position;
                focusCamera.transform.rotation = targetCameraTransform.rotation;

                focusCameraCurrentState = focusCameraState.focused;
            }

        }
    }

    public void InteractWithNPC(NPC npc)
    {
        interactingWith = npc;

        string dialogueLine = npc.ReturnDialoguePoint();

        if(dialogueLine != null)
        {
            StartFocusCameraMove(npc.cameraFocusPoint);

            playerMove.StopMoving();
            UI.ShowDialogue(dialogueLine);
        }
        else
        {
            UI.HideDialogue();
            StartFocusCameraMove(npc.cameraFocusPoint);
            //interactingWith.ResetDialogue();

            playerMove.StopMoving();

            if (interactingWith.isQuestGiver)
            {
                if (qmanager.InteractWith(interactingWith.npcName))
                {
                    return;
                }
            }

            EndConversation();
            StopFocusCamera();

        }

    }

    public void ProgressDialogue()
    {
        string dialogueLine = interactingWith.ReturnDialoguePoint();

        if (dialogueLine != null)
        {
            UI.ShowDialogue(dialogueLine);
        }
        else
        {
            UI.HideDialogue();
            interactingWith.ResetDialogue();

            if (interactingWith.isQuestGiver)
            {
                if (qmanager.InteractWith(interactingWith.npcName))
                {
                    return;
                }
            }

            EndConversation();
            StopFocusCamera();
            
            
        }
    }

    public void EndConversation()
    {
        playerMove.StartMoving();
        UI.HideDialogue();
        
        interactingWith.ResetDialogue();

        StopFocusCamera();
    }

    private void StartFocusCameraMove(Transform target)
    {
        Debug.Log("start focus cam");
        playerCamera.SetActive(false);

        focusCamera.transform.position = playerCamera.transform.position;
        focusCamera.transform.rotation = playerCamera.transform.rotation;

        focusCamera.SetActive(true);
        targetCameraTransform = target;
        focusCameraCurrentState = focusCameraState.moving;
    }
    public void StopFocusCamera()
    {
        playerCamera.SetActive(true);
        focusCamera.SetActive(false);
        targetCameraTransform = null;
        focusCameraCurrentState = focusCameraState.normal;

        Debug.Log("Stop focus cam");
    }

    
}
