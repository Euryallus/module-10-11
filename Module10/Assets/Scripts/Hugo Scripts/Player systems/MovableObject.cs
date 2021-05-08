using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main author:         Hugo Bailey
// Additional author:   N/A
// Description:         Used by the Grab ability (currently tied to the Pickaxe) to pick up & move physics objects around
// Development window:  Prototype phase
// Inherits from:       MonoBehaviour

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : MonoBehaviour
{
    [HideInInspector]   public bool isHeld;         // Flags if player is currently holding the object
    [SerializeField]    private Transform target;   // Ref. to transform of the player's ""hand"" - position the object moves towards
    [SerializeField]    private Rigidbody rb;       // Ref. to own RigidBody component
    private Rigidbody handTarget;
    private SpringJoint joint;

    [SerializeField] private float jointSpring = 5f;
    [SerializeField] private float jointDamper = 10f;

    [SerializeField] private float movementDampen = 0.3f;

    private Vector3 currentVelocity = Vector3.zero;

    private bool isTouchingOther = false;

    void Start()
    {
        isHeld = false;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // If the player is currently holding the object, move towards the players hand position
        if (isHeld && !isTouchingOther)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentVelocity, movementDampen);
        }

        if(!gameObject.GetComponent<SpringJoint>())
        {
            transform.parent = null;
            isHeld = false;
            rb.useGravity = true;
        }
    }

    // Sets target position to players hand, turns off grav & sets isHeld to true
    public void PickUp(Transform hand)
    {
        handTarget = hand.transform.gameObject.GetComponent<Rigidbody>();
        transform.parent = hand.transform;

        joint = gameObject.AddComponent<SpringJoint>();
        joint.spring = jointSpring;
        joint.damper = jointDamper;

        joint.connectedBody = handTarget;

        rb.constraints = RigidbodyConstraints.FreezeRotation;


        // ""Resets"" velocity when object is held to avoid issues when moving around
        rb.velocity = Vector3.zero;
        // Sets target to the player's "hand" and disables gravity on the object, flags isHeld
        target = hand;
        rb.useGravity = false;
        isHeld = true;
    }

    // Sets item down where it is and re-enables grav.
    public void DropObject(Vector3 direction)
    {
        rb.constraints = RigidbodyConstraints.None;

        transform.parent = null;
        Destroy(joint);
        isHeld = false;
        rb.useGravity = true;
    }

    // Throws object in the direction the player is facing, re-enables grav etc.
    public void ThrowObject(Vector3 direction)
    {
        rb.constraints = RigidbodyConstraints.None;

        transform.parent = null;
        Destroy(joint);
        isHeld = false;
        rb.useGravity = true;
        rb.AddForce(direction.normalized * 600);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isTouchingOther = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingOther = false;
    }
}
