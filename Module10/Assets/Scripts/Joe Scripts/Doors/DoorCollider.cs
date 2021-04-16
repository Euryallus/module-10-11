using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : InteractableWithOutline
{
    [Header("Door Collider")]

    [SerializeField] private DoorMain doorMainScript;

    public override void Interact()
    {
        base.Interact();

        doorMainScript.Interact();
    }
}
