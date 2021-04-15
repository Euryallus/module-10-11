using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldPlaceableItem : HeldItem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
