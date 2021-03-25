using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : DestructableObject
{
    public Rigidbody topSection;

    public override void Destroyed()
    {
        topSection.useGravity = true;
        topSection.AddExplosionForce(100f, transform.position, 20f);
    }
}
