using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedObject : MonoBehaviour
{
    public List<Rigidbody> bodies;
    public float force = 20f;

    //adds an explosion force on all fractured parts of the object to make it go boom
    public void Explode()
    {
        for(int i = 0; i <bodies.Count; i++)
        {
            bodies[i].AddExplosionForce(20f, transform.position, force);
        }

        StartCoroutine("Despawn");
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }


}
