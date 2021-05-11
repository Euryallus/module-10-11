using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : CollectableResource
{
    [SerializeField] private Rigidbody treeTop;

    public override void TryToDestroy()
    {
        base.TryToDestroy();

        if(toBeDestroyed)
        {
            treeTop.velocity = Vector3.zero;
            Vector3 forceDir = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position) * 300;

            treeTop.AddForce(forceDir);
        }
    }
}
