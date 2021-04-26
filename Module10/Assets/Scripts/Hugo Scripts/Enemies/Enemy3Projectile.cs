using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float launchVelocity;


    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Launch(Vector3 direction, Transform target)
    {
        rb.constraints = RigidbodyConstraints.None;
        //https://forum.unity.com/threads/find-the-angle-to-hit-target-at-x-y-z.33659/

        transform.LookAt(target.transform.position);
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 3), Color.green, 3f);


        Vector3 targetVect = transform.InverseTransformDirection(direction);

        targetVect.z *= -1;

        //Debug.Log(targetVect);

        //targetVect = transform.InverseTransformDirection(targetVect);

        float x = targetVect.z;
        float y = targetVect.y;
        float v = launchVelocity;
        float g = -9.81f;

        float v2 = v * v;
        float v4 = v2 * v2;
        float x2 = x * x;

        float theta = Mathf.Atan2(v2 - Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2)), g * x);

        transform.Rotate(new Vector3(-theta * Mathf.Rad2Deg, 0, 0));

        rb.velocity = launchVelocity * transform.forward;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int mask = 1 << 6;
        if (collision.transform.gameObject.layer != mask)
        {
            Die();
        }
    }

}
