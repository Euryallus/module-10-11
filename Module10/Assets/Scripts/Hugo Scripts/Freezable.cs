using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezable : MonoBehaviour
{
    public Rigidbody attachedRB;

    public RigidbodyConstraints originalRBConstraints;

    public void Freeze()
    {
        originalRBConstraints = attachedRB.constraints;

        attachedRB.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreeze()
    {
        attachedRB.constraints = originalRBConstraints;
    }
}
