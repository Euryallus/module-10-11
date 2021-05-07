using UnityEngine;

// ||=======================================================================||
// || DoorTriggerArea: Detects when the player is on either side of a door. ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

public class DoorTriggerArea : MonoBehaviour
{
    #region InspectorVariables
    // Variables in this region are set in the inspector

    [Header("Door Trigger Area")]

    [SerializeField] private DoorMain   doorMainScript; // The main door script that handles door logic
    [SerializeField] private bool       inside;         // Whether this trigger area is in the 'inside' side of the door (true), or 'outside' (false)

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        // The player entered the area, tell the main door script
        doorMainScript.TriggerEntered(inside);
    }

    private void OnTriggerExit(Collider other)
    {
        // The player left the area, tell the main door script
        doorMainScript.TriggerExited(inside);
    }
}