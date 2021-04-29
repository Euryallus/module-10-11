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

    [Header("Object health")] [SerializeField]
    protected int hitsToBreak = 3;

    [Header("Particles")]
    [SerializeField] private GameObject destroyParticlesPrefab; //Prefab containing particles to be spawned when the piece is hit
    [SerializeField] private Transform destroyParticlesTransform;

    [Header("Hit Sound")]
    [SerializeField] private SoundClass hitSound; //Sound played when the object is hit

    protected int health;

    bool destroyed = false;

    protected virtual void Start()
    {
        // sets health to default
        health = hitsToBreak;
    }

    public virtual void TakeHit() 
    {
        //reduces "health" of resource object

        --health;

        if(hitSound != null)
        {
            AudioManager.Instance.PlaySoundEffect3D(hitSound, transform.position);
        }

        if(health <= 0)
        {
            Destroyed();
        }
    }

    public virtual void Destroyed()
    {
        //adds item dropped to inventory
        InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();
        foreach(ItemGroup stack in itemDroppedOnDestroy)
        {
            for (int i = 0; i < stack.Quantity; i++)
            {
                inventory.AddItemToInventory(stack.Item);
                //flagged destroyed as true
                destroyed = true;
            }
        }

        if(destroyParticlesPrefab != null)
        {
            ParticleGroup particleGroup = Instantiate(destroyParticlesPrefab, destroyParticlesTransform.position, Quaternion.identity).GetComponent<ParticleGroup>();

            particleGroup.PlayEffect();
        }
    }
}
