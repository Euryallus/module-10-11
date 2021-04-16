using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : InteractableWithOutline
{
    public override void Interact()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().InteractWithLadder();
        base.Interact();
    }
}
