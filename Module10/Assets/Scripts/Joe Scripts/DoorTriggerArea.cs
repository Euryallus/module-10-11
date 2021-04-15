using UnityEngine;

public class DoorTriggerArea : MonoBehaviour
{
    [Header("Door Trigger Area")]

    [SerializeField] private DoorMain doorMainScript;
    [SerializeField] private bool inside;

    private void OnTriggerEnter(Collider other)
    {
        doorMainScript.TriggerEntered(inside);
    }

    private void OnTriggerExit(Collider other)
    {
        doorMainScript.TriggerExited(inside);
    }
}
