using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMain : MonoBehaviour
{
    [Header("Door Main")]

    [SerializeField] private Animator   animator;
    [SerializeField] private float      closeAfterTime = 5.0f;

    private bool openIn;
    private bool openOut;

    private bool inInsideTrigger;
    private bool inOutsideTrigger;

    private float doorOpenTimer;

    private void Update()
    {
        if(openIn || openOut)
        {
            doorOpenTimer += Time.deltaTime;

            //Door has been open for a while and the player isn't standing in the way of it closing
            if(doorOpenTimer >= closeAfterTime &&
                ((openIn && !inInsideTrigger) || (openOut && !inOutsideTrigger)) )
            {
                SetAsClosed();
            }
        }
    }

    public void Interact()
    {
        if (!openIn && !openOut)
        {
            if (inInsideTrigger)
            {
                SetAsOpen(false);
            }
            else if(inOutsideTrigger)
            {
                SetAsOpen(true);
            }
        }
        else
        {
            SetAsClosed();
        }
    }

    public void TriggerEntered(bool inside)
    {
        inInsideTrigger = inside;
        inOutsideTrigger = !inside;
    }

    public void TriggerExited(bool inside)
    {
        if (inside)
        {
            inInsideTrigger = false;
        }
        else
        {
            inOutsideTrigger = false;
        }
    }

    private void SetAsOpen(bool inwards)
    {
        doorOpenTimer = 0.0f;

        openIn  = inwards;
        openOut = !inwards;

        if (inwards)
        {
            animator.SetBool("OpenIn", true);
        }
        else
        {
            animator.SetBool("OpenOut", true);
        }
    }

    private void SetAsClosed()
    {
        openIn = false;
        openOut = false;

        animator.SetBool("OpenIn", false);
        animator.SetBool("OpenOut", false);
    }
}
