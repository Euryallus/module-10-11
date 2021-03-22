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
        if (PlayerIsWithinRange())
        {
            if(mouseOver && !hoveringInRange)
            {
                hoveringInRange = true;
                StartHover();
            }

            if (pressEToInteract && mouseOver && Input.GetKeyDown(KeyCode.E) && !CustomInputField.AnyFieldSelected)
            {
                Interact();
            }
        }
        else
        {
            if (hoveringInRange)
            {
                hoveringInRange = false;
                EndHover();
            }
        }
    }

    private void OnMouseOver()
    {
        //Right click
        if (rightClickToInteract && Input.GetMouseButtonDown(1) && PlayerIsWithinRange())
        {
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