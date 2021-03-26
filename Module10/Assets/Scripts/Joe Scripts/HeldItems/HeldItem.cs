using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public bool PerformingPuzzleAbility { get { return performingPuzzleAbility; } }

    protected bool performingPuzzleAbility;
    protected Item item;

    public void Setup(Item item)
    {
        this.item = item;
    }

    public virtual void PerformMainAbility()
    {
        //For example, break a rock
    }

    public virtual void StartPuzzleAbility()
    {
        Debug.Log("Starting puzzle ability");
        performingPuzzleAbility = true;

        //For example, pick up and start moving an object
    }

    public virtual void EndPuzzleAbility()
    {
        Debug.Log("Ending puzzle ability");
        performingPuzzleAbility = false;

        //For example, drop an object
    }

    private void OnDestroy()
    {
        //If the player is performing an ability when this held item is destroyed,
        //  make sure the puzzle ability behaviour is stopped
        if (performingPuzzleAbility)
        {
            EndPuzzleAbility();
        }
    }
}