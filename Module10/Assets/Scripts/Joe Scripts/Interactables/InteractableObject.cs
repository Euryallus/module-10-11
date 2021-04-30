using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InteractableObject : MonoBehaviour
{
    [Header("Interactable Properties")]
    [Tooltip("How close the player needs to be to this object to interact with it")]
    [SerializeField] private float      interactionRange        = 5.0f;
    [SerializeField] private bool       pressEToInteract        = true;
    [SerializeField] private bool       rightClickToInteract    = true;
    [SerializeField] private GameObject interactTooltipPrefab;
    [SerializeField] private Vector3    interactTooltipOffset;

    private bool        mouseOver;
    private bool        hoveringInRange;
    private GameObject  playerGameObject;
    private Transform   canvasTransform;
    private GameObject  interactTooltip;
    private float       hoverTimer;
    private Vector3     localInteractTooltipOffset;
    private Camera      mainPlayerCamera;

    private const float InteractPopupDelay = 0.3f;

    protected virtual void Start()
    {
        playerGameObject    = GameObject.FindGameObjectWithTag("Player");
        canvasTransform     = GameObject.FindGameObjectWithTag("JoeCanvas").transform;

        mainPlayerCamera = Camera.main;

        localInteractTooltipOffset = transform.InverseTransformDirection(interactTooltipOffset);
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

        if(hoveringInRange)
        {
            hoverTimer += Time.unscaledDeltaTime;

            if(hoverTimer >= InteractPopupDelay && interactTooltip == null && interactTooltipPrefab != null)
            {
                interactTooltip = Instantiate(interactTooltipPrefab, canvasTransform);
            }
        }

        if(interactTooltip != null && mainPlayerCamera != null)
        {
            interactTooltip.transform.position = mainPlayerCamera.WorldToScreenPoint(transform.position + localInteractTooltipOffset);
        }
    }

    private void OnMouseOver()
    {
        //Right click
        if (rightClickToInteract && Input.GetMouseButtonDown(1) && PlayerIsWithinRange() && Cursor.lockState == CursorLockMode.Locked)
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
        EndHover();
    }

    private bool PlayerIsWithinRange()
    {
        if((Vector3.Distance(playerGameObject.transform.position, transform.position) <= interactionRange) && !EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    public virtual void Interact()
    {
        EndHover();
    }

    public virtual void StartHover()
    {
        hoverTimer = 0.0f;
    }

    public virtual void EndHover()
    {
        hoverTimer = 0.0f;
        hoveringInRange = false;

        if (interactTooltip != null)
        {
            Destroy(interactTooltip);
        }
    }
}