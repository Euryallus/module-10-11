using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTree : DestructableObject
{
    public Rigidbody topSection;

    public GameObject stump;
    public Transform leavesPos;

    private GameObject newStump;

    bool canDestroy = false;

    private void Start()
    {
        health = hitsToBreak;
    }

    private void Update()
    {
        if(topSection.velocity.magnitude < 0.1f && canDestroy)
        {
            Destroy(topSection.gameObject);
        }
    }

    public override void Destroyed()
    {
        topSection.useGravity = true;
        topSection.constraints = RigidbodyConstraints.None;

        //topSection.AddExplosionForce(100f, transform.position, 20f);

        Vector3 forceDir = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).normalized * 100;

        topSection.AddForce(forceDir);

        StartCoroutine(nameof(DestroyTopSection));

        newStump = Instantiate(stump);
        newStump.transform.position = transform.position;

        base.Destroyed();
    }

    private IEnumerator DestroyTopSection()
    {
        yield return new WaitForSeconds(3);

        newStump.GetComponent<MeshCollider>().enabled = true;
        canDestroy = true;
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        ParticleManager.Instance.SpawnParticle(leavesPos.position, "Trees");
    }


}
