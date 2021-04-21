using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : DestructableObject
{
    // test - prefab of "fractured" version of object to crumble when obj. is destroyed
    public GameObject fracturedVersion;
    public GameObject wholeVersion;

    public bool CanBeHit = true;
    public GameObject hammerContainer;
    public Animator hammer;

    public override void Destroyed()
    {
        if (fracturedVersion != null)
        {
            //create new "fractured" version of the object and explode it

            fracturedVersion.SetActive(true);
            fracturedVersion.GetComponent<FracturedObject>().Explode();

            wholeVersion.SetActive(false);
            if (gameObject.GetComponent<MeshCollider>())
            {
                gameObject.GetComponent<MeshCollider>().enabled = false;
            }
            else if (gameObject.GetComponent<BoxCollider>())
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }

        }
    }

    public override void TakeHit()
    {
        if (CanBeHit)
        {
            base.TakeHit();


            hammerContainer.transform.forward = GameObject.FindGameObjectWithTag("Player").transform.forward;


            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopMoving();
            CanBeHit = false;
            hammer.SetBool("Swing", true);

            GameObject.FindGameObjectWithTag("Hammer").GetComponent<MeshRenderer>().enabled = false;
        }

    }

}

