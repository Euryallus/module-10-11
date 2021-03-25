using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("NPC data")]
    public string npcName;
    public bool isQuestGiver = false;

    [Header("Dialogue data")]
    public string[] dialogueLines;
    private int dialogueProgression = 0;

    //flags if dialogue should repeat
    [HideInInspector]
    public bool haveTalkedToBefore = false;

    [Header("Focus camera point")]
    public Transform cameraFocusPoint;

    public string ReturnDialoguePoint()
    {
        if(dialogueProgression < dialogueLines.Length)
        {
            // each call returns next dialogue line
            string returnString = dialogueLines[dialogueProgression];
            ++dialogueProgression;

            return returnString;
        }
        else
        {
            //if end of dialogue is met and npc is a quest giver, dont repeat the lines
            if(isQuestGiver)
            {
                haveTalkedToBefore = true;
            }

        }

        return null;
    }

    public void ResetDialogue()
    {
        //called when player exits conversation, checks if "point in conversation" needs returning to 0
        if(!haveTalkedToBefore)
        {
            dialogueProgression = 0;
        }

    }

}
