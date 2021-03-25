using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractableObject : MonoBehaviour
{
    [Header("Interactable Properties")]
    [Tooltip("How close the player needs to be to this object to interact with it")]
    [SerializeField] private float  interactionRange        = 5.0f;
    [SerializeField] private bool   pressEToInteract        = true;
    [SerializeField] private bool   rightClickToInteract    = true;

    private bool        mouseOver;
    private bool        hoveringInRange;
    private GameObject  playerGameObject;

    protected virtual void Start()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Update()
    {
        //Interabtables can only be interacted with when the cursor is locked (i.e. the player is moving around the world/not in a menu)
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            //Check if the player is within the set interaction range
            if (PlayerIsWithinRange())
            {
                if (mouseOver && !hoveringInRange)
                {
                    //Player is in range and their mouse is over the object, start hovering
                    hoveringInRange = true;
                    StartHover();
                }

                if (pressEToInteract && mouseOver && Input.GetKeyDown(KeyCode.E) && !CustomInputField.AnyFieldSelected)
                {
                    //Ensure no input field is selected to prevent pressing E while typing causing unintended behaviour
                    //The player has pressed E while hovering over the object, interact with it
                    Interact();
                }
            }
            else
            {
                if (hoveringInRange)
                {
                    //Player is no longer in range, end hovering
                    EndHover();
                }
            }
        }
    }

    private void OnMouseOver()
    {
        //Right click
        if (rightClickToInteract && Input.GetMouseButtonDown(1) && PlayerIsWithinRange())
        {
            //The player has right clicked on the object while in range, interact with it
            Interact();
        }
    }

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        hoveringInRange = false;
        EndHover();
    }

    private bool PlayerIsWithinRange()
    {
        if(Vector3.Distance(playerGameObject.transform.position, transform.position) <= interactionRange)
        {
            return true;
        }
        return false;
    }

    public abstract void Interact();

    public abstract void StartHover();

    public abstract void EndHover();
}