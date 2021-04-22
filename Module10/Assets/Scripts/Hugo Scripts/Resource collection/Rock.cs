using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : DestructableObject
{
    // test - prefab of "fractured" version of object to crumble when obj. is destroyed
    public GameObject fracturedVersion;
    public GameObject wholeVersion;

    public bool CanBeHit = true;
    public GameObject pickaxeContainer;
    public Animator pickaxe;

    public override void Destroyed()
    {
        if (fracturedVersion != null)
        {
            //create new "fractured" version of the object and explode it

            fracturedVersion.SetActive(true);
            fracturedVersion.GetComponent<FracturedObject>().Explode();

            wholeVersion.SetActive(false);
            if(gameObject.GetComponent<MeshCollider>())
            {
                gameObject.GetComponent<MeshCollider>().enabled = false;
            }
            else if(gameObject.GetComponent<BoxCollider>())
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }

        }
        base.Destroyed();
    }

    public override void TakeHit()
    {
        if (CanBeHit)
        { 
            base.TakeHit();

            
            pickaxeContainer.transform.forward = GameObject.FindGameObjectWithTag("Player").transform.forward;

           
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopMoving();
            CanBeHit = false;
            pickaxe.SetBool("Swing", true);
            
            GameObject.FindGameObjectWithTag("Pickaxe").GetComponent<MeshRenderer>().enabled = false;
        }

    }

}
