using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : DestructableObject
{
    // test - prefab of "fractured" version of object to crumble when obj. is destroyed
    public GameObject fracturedVersion = null;

    public override void Destroyed()
    {
        if (fracturedVersion != null)
        {
            //create new "fractured" version of the object and explode it

            GameObject fract = Instantiate(fracturedVersion);
            fract.transform.position = gameObject.transform.position;
            fract.transform.rotation = gameObject.transform.rotation;

            fract.GetComponent<FracturedObject>().Explode();

            base.Destroyed();

            Destroy(gameObject);
        }
    }

}
