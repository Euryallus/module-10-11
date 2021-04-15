using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableWithOutline
{
    [SerializeField] private Animator animator;

    private bool openIn;
    private bool openOut;

    public override void Interact()
    {
        base.Interact();

        if(!openIn && !openOut)
        {
            SetAsOpenIn();
        }
        else
        {
            SetAsClosed();
        }
    }

    private void SetAsOpenIn()
    {
        openIn = true;
        animator.SetBool("OpenIn", true);
    }

    private void SetAsOpenOut()
    {
        openOut = true;
        animator.SetBool("OpenOut", true);
    }

    private void SetAsClosed()
    {
        openIn = false;
        openOut = false;
        animator.SetBool("OpenIn", false);
        animator.SetBool("OpenOut", false);
    }
}
