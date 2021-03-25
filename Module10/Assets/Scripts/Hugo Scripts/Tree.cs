using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : DestructableObject
{
    public Rigidbody topSection;

    public override void Destroyed()
    {
        topSection.useGravity = true;
        topSection.constraints = RigidbodyConstraints.None;

        //topSection.AddExplosionForce(100f, transform.position, 20f);

        Vector3 forceDir = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position ).normalized * 200;

        topSection.AddForce(forceDir);

        base.Destroyed();
    }
}
