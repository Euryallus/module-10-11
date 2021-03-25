using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Quest manager ref")]
    private QuestManager qmanager;

    private DialogueUI UI;

    private NPC interactingWith = null;
    private PlayerMovement playerMove;

    private focusCameraState focusCameraCurrentState = focusCameraState.normal;

    private Transform targetCameraTransform;
    private enum focusCameraState
    { 
        normal,
        moving,
        focused
    }

    [Header("Camera focus componens")]
    // allows 2nd camera to focus on NPC
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private GameObject focusCamera;
    //speed camera lerps from current pos to focused pos
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
        // checks if camera should be moving
        if(focusCameraCurrentState == focusCameraState.moving)
        {
            // LERP from current pos to target pos over [1 / cameraLerpSpeed] seconds
            focusCamera.transform.position = Vector3.Lerp(focusCamera.transform.position, targetCameraTransform.position, cameraLerpSpeed * Time.deltaTime);
            focusCamera.transform.rotation = Quaternion.Lerp(focusCamera.transform.rotation, targetCameraTransform.rotation, cameraLerpSpeed * Time.deltaTime);

            // checks if camera is close enough to target, if so stop moving & snap to target pos
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

        // checks if NPC has any dialogue to say - if so, focus on target & show dialogue UI
        if(dialogueLine != null)
        {
            StartFocusCameraMove(npc.cameraFocusPoint);

            playerMove.StopMoving();
            UI.ShowDialogue(dialogueLine);
        }
        else
        // checks if NPC has any quests to give instead
        {
            UI.HideDialogue();
            
            StartFocusCameraMove(npc.cameraFocusPoint);

            // prevents player from using movement input while talking to someone
            playerMove.StopMoving();

            if (interactingWith.isQuestGiver)
            {
                if (qmanager.InteractWith(interactingWith.npcName))
                {
                    //if the NPC is a quest giver and has something to say to the player, do that instead of this
                    return;
                }
            }

            // if npc has nothing to say at all, end the convo and stop focusing camera
            EndConversation();
            StopFocusCamera();

        }

    }

    public void ProgressDialogue()
    {
        // called by "Next" button on dialogue UI
        string dialogueLine = interactingWith.ReturnDialoguePoint();

        if (dialogueLine != null)
        {
            // if the NPC has something else to say, show it
            UI.ShowDialogue(dialogueLine);
        }
        else
        {
            // if NPC has no dialogue left, check for quests to give / complete
            UI.HideDialogue();
            interactingWith.ResetDialogue();

            if (interactingWith.isQuestGiver)
            {
                if (qmanager.InteractWith(interactingWith.npcName))
                {
                    // if npc has quests to accept / give, do that instead of this
                    return;
                }
            }

            // if nothing left to say, end the convo & defocus camera
            EndConversation();
            StopFocusCamera();
            
            
        }
    }

    public void EndConversation()
    {
        // called when player hits "leave" button on dialogue UI

        //allows player to move again, hides dialogue UI, resets dialogue and returns cam control to the player
        playerMove.StartMoving();
        UI.HideDialogue();
        
        interactingWith.ResetDialogue();

        StopFocusCamera();
    }

    private void StartFocusCameraMove(Transform target)
    {
        // de-activates player camera
        playerCamera.SetActive(false);

        // assigns initial position of focus cam to match players current position
        focusCamera.transform.position = playerCamera.transform.position;
        focusCamera.transform.rotation = playerCamera.transform.rotation;

        // activates focus cam 
        focusCamera.SetActive(true);

        //sets target transform for focus cam
        targetCameraTransform = target;
        //sets movement state to "moving"
        focusCameraCurrentState = focusCameraState.moving;
    }
    public void StopFocusCamera()
    {
        // re-activates player camera & de-activates focus cam
        playerCamera.SetActive(true);
        focusCamera.SetActive(false);

        // sets target transform to null, sets camera mode to normal
        targetCameraTransform = null;
        focusCameraCurrentState = focusCameraState.normal;
    }

    
}
