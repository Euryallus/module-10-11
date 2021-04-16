using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTree : DestructableObject
{
    public Rigidbody topSection;

    public GameObject stump;

    private void Start()
    {
        health = hitsToBreak;
    }

    public override void Destroyed()
    {
        topSection.useGravity = true;
        topSection.constraints = RigidbodyConstraints.None;

        //topSection.AddExplosionForce(100f, transform.position, 20f);

        Vector3 forceDir = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).normalized * 100;

        topSection.AddForce(forceDir);

        StartCoroutine(nameof(DestroyTopSection));

        GameObject newStump = Instantiate(stump);
        newStump.transform.position = transform.position;

        base.Destroyed();
    }

    private IEnumerator DestroyTopSection()
    {
        Debug.Log("WEE");
        yield return new WaitForSeconds(3);

        Destroy(topSection.gameObject);
    }


}
