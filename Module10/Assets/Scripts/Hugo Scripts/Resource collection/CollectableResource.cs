using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableResource : DestructableObject
{
    [SerializeField] private GameObject[] enableOnDestruction;
    [SerializeField] private GameObject[] disableOnDestruction;

    [SerializeField] private GameObject toolHolder;
    [SerializeField] private Animator toolAnimator;

    [SerializeField] private Collider colliderDisableOnDestroy;

    [HideInInspector] public bool canBeHit = true;
    public MeshRenderer toolRenderer;

    public bool toBeDestroyed = false;

    public override void TakeHit()
    {
        if(canBeHit)
        {
            toolHolder.transform.forward = GameObject.FindGameObjectWithTag("Player").transform.forward;

            toolRenderer = GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(1).GetComponent<HeldTool>().toolRenderer;
            Debug.Log(toolRenderer.name);

            toolRenderer.enabled = false;

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopMoving();
            toolAnimator.SetTrigger("Swing");

            base.TakeHit();
        }
    }

    public override void Destroyed()
    {
        toBeDestroyed = true;
    }

    public virtual void TryToDestroy()
    {
        if(toBeDestroyed)
        {
            base.Destroyed();

            foreach (GameObject obj in disableOnDestruction)
            {
                obj.SetActive(false);
            }
            foreach(GameObject obj in enableOnDestruction)
            {
                obj.SetActive(true);

                if(obj.GetComponent<FracturedObject>())
                {
                    obj.GetComponent<FracturedObject>().Explode();
                }
            }

            if(colliderDisableOnDestroy != null)
            {
                colliderDisableOnDestroy.enabled = false;
            }
        }
    }
}

