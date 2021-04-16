using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//new better trees!
public class NewTree : DestructableObject
{
    public Rigidbody    topSection; //Rigidbody of top section

    public Transform    leavesPos;  //point where leaves should appear when hit (visuals)

    private GameObject  newStump;   //stump spawned when tree falls
    public GameObject   stump;      //stump prefab

    bool canDestroy = false;

    private void Update()
    {
        //checks if top can be deleted (is stationary and has been cut down)
        if(topSection.velocity.magnitude < 0.1f && canDestroy)
        {
            Destroy(topSection.gameObject);
        }
    }

    public override void Destroyed()
    {
        //causes top section of tree to fall
        topSection.useGravity = true;
        topSection.constraints = RigidbodyConstraints.None;

        //topSection.AddExplosionForce(100f, transform.position, 20f);

        //applies force to tree in direction opposite to direction hit from
        Vector3 forceDir = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).normalized * 100;

        topSection.AddForce(forceDir);

        //begins co-routine
        StartCoroutine(nameof(DestroyTopSection));

        //creates stump of tree
        newStump = Instantiate(stump);
        newStump.transform.position = transform.position;

        base.Destroyed();
    }

    private IEnumerator DestroyTopSection()
    {
        yield return new WaitForSeconds(3);
         
        //after 3 seconds flag top for deletion and enable collider on stump
        newStump.GetComponent<MeshCollider>().enabled = true;
        canDestroy = true;
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        //causes leaves to shake out when hits other objects
        ParticleManager.Instance.SpawnParticle(leavesPos.position, "Trees");
    }


}
