using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedObject : MonoBehaviour
{
    public List<Rigidbody> bodies;

    //adds an explosion force on all fractured parts of the object to make it go boom
    public void Explode()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(100f, transform.position, 20f);
        } 
    }
}
