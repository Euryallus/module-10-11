using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;
    public bool isQuestGiver = false;

    public string[] dialogueLines;
    private int dialogueProgression = 0;

    [HideInInspector]
    public bool haveTalkedToBefore = false;

    public Transform cameraFocusPoint;

    public string ReturnDialoguePoint()
    {
        if(dialogueProgression < dialogueLines.Length)
        {
            string returnString = dialogueLines[dialogueProgression];
            ++dialogueProgression;

            return returnString;
        }
        else
        {
            if(isQuestGiver)
            {
                haveTalkedToBefore = true;
            }

        }

        return null;
    }

    public void ResetDialogue()
    {
        if(!haveTalkedToBefore)
        {
            dialogueProgression = 0;
        }

    }

}
