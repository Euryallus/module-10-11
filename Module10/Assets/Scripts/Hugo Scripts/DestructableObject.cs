using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//allows object to be destroyed for items
public class DestructableObject : MonoBehaviour
{
    [Header("Items associated with object")]
    // Items used to destroy object & items collected when destroyed
    public ItemGroup[] itemDroppedOnDestroy;
    public Item[] toolToBreak;

    [Header("Object health")]
    private int hitsToBreak = 3;
    private int health;

    // test - prefab of "fractured" version of object to crumble when obj. is destroyed
    public GameObject fracturedVersion = null;


    private void Start()
    {
        // sets health to default

        health = hitsToBreak;
    }

    public virtual void TakeHit() 
    {
        --health;

        if(health == 0)
        {
            Destroyed();
        }
    }

    public virtual void Destroyed()
    {
        if(fracturedVersion != null)
        {
            //create new "fractured" version of the object and explode it
            GameObject fract = Instantiate(fracturedVersion);
            fract.transform.position = gameObject.transform.position;
            fract.transform.rotation = gameObject.transform.rotation;
            
            fract.GetComponent<FracturedObject>().Explode();
            Destroy(gameObject);
        }
    }
}
