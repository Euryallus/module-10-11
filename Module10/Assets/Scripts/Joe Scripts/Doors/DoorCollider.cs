using UnityEngine;

//DoorCollider is a script that should be attached to the GameObject containing a collider for opening/closing the door

public class DoorCollider : InteractableWithOutline
{
    [Header("Door Collider")]

    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private DoorMain doorMainScript;   //The main script attached to the door than handles opening/closing events

    #endregion

    public override void Interact()
    {
        base.Interact();

        //Tell the door to open/close/notify the player if they cannot open it
        doorMainScript.Interact();
    }
}
