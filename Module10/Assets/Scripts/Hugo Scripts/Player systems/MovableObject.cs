using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attached to objects that can be picked up & moved (REQUIRES A RIGIDBODY FOR THIS TO WORK)
public class MovableObject : MonoBehaviour
{
    //bool value (toggled if player is holding the item)
    [SerializeField]
    public bool isHeld;

    //saves "hand" position of player
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 targetPos;

    //stores ref. to Rigidbody component
    [SerializeField]
    private Rigidbody rb;


    void Start()
    {
        isHeld = false;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //if the player is currently holding the object, move towards the players hand position
        if (isHeld)
        {
            targetPos = target.position;
            transform.position = Vector3.Lerp(transform.position, target.position, 5 * Time.deltaTime);
        }
    }

    //sets target position to players hand, turns off grav & sets _isHeld to true
    public void PickUp(Transform hand)
    {
        rb.velocity = Vector3.zero;

        target = hand;
        rb.useGravity = false;
        isHeld = true;
    }

    //sets item down where it is and re-enables grav.
    public void DropObject(Vector3 direction)
    {
        isHeld = false;
        rb.useGravity = true;
    }

    //throws object in the direction the player is facing, re-enables grav etc.
    public void ThrowObject(Vector3 direction)
    {
        isHeld = false;
        rb.useGravity = true;
        rb.AddForce(direction.normalized * 600);
    }
}
