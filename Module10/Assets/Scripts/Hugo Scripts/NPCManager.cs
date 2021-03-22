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

    private void Start()
    {
        UI = gameObject.GetComponent<DialogueUI>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        UI.HideDialogue();
    }

    public void InteractWithNPC(NPC npc)
    {
        interactingWith = npc;

        string dialogueLine = npc.ReturnDialoguePoint();

        if(dialogueLine != null)
        {
            playerMove.StopMoving();
            UI.ShowDialogue(dialogueLine);
        }
        else
        {
            if(npc.isQuestGiver)
            {
                qmanager.InteractWith(npc.npcName);
            }
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
            EndConversation();

            if (interactingWith.isQuestGiver)
            {
                qmanager.InteractWith(interactingWith.npcName);
            }
        }
    }

    public void EndConversation()
    {
        playerMove.StartMoving();
        UI.HideDialogue();

        interactingWith.ResetDialogue();
    }
}
