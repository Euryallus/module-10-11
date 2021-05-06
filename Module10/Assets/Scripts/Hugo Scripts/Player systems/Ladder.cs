using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main author:         Hugo Bailey
// Additional author:   Base class written by Joe Allen
// Description:         Manages NPC interactions & passes data to QuestManager when NPC is also a quest giver. Communicates with DialogueUI to display dialogue on screen
// Development window:  Prototype phase
// Inherits from:       InteractableWithOutline

public class Ladder : InteractableWithOutline
{
    public override void Interact()
    {
        // Flags ladder as having been interacted with in PlayerMovement script
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().InteractWithLadder();
        base.Interact();
    }
}
