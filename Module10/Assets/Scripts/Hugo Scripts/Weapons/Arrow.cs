using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damageDone;

    private bool hasHit = false;

    private void Awake()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0.875f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(!hasHit && !collision.transform.CompareTag("Player"))
        {
            if(collision.transform.gameObject.isStatic)
            {
                gameObject.transform.parent = collision.gameObject.transform;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }

            if(collision.gameObject.GetComponent<EnemyHealth>())
            {
                gameObject.transform.parent = collision.gameObject.transform;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                collision.gameObject.GetComponent<EnemyHealth>().DoDamage(damageDone);
                Destroy(gameObject);
            }
            hasHit = true;
        }

    }
}
