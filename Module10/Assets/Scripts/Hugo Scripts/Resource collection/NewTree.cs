using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//new better trees!
public class NewTree : DestructableObject
{
    public Rigidbody    topRB; //Rigidbody of top section

    public GameObject   stump;      //stump prefab
    public GameObject   top;

    public GameObject staticTree;

    public GameObject axeContainer;
    public Animator axe;
    public bool CanBeHit = true;

    bool canDestroy = false;

    private void Update()
    {
        //checks if top can be deleted (is stationary and has been cut down)
        if(top.activeSelf)
        {
            //if(topRB.velocity.magnitude < 0.1f && canDestroy)
            //{
            //    top.SetActive(false);
            //}
            
        }
    }

    public override void TakeHit()
    {
        if(CanBeHit)
        {
            axeContainer.transform.forward = GameObject.FindGameObjectWithTag("Player").transform.forward;

            base.TakeHit();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopMoving();
            CanBeHit = false;
            axe.SetBool("Swing", true);

            GameObject.FindGameObjectWithTag("Axe").GetComponent<MeshRenderer>().enabled = false;
        }

    }

    public override void Destroyed()
    {
        staticTree.SetActive(false);

        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        stump.SetActive(true);
        top.SetActive(true);

        //applies force to tree in direction opposite to direction hit from
        Vector3 forceDir = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position) * 300;

        topRB.AddForce(forceDir);

        //begins co-routine
        StartCoroutine(nameof(DestroyTopSection));
        base.Destroyed();
    }

    private IEnumerator DestroyTopSection()
    {
        yield return new WaitForSeconds(6);
        
        canDestroy = true;
    }



}
