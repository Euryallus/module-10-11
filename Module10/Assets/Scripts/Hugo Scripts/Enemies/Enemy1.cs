using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : EnemyBase
{
    public GameObject duplicatePrefab;
    public List<GameObject> children;

    private bool HasSplit = false;
    public int numberOfDuplicates;
    public float duplicateSpawnDistance;

    public override void Attack()
    {
        if(!HasSplit)
        {
            for (int i = 0; i < numberOfDuplicates; i++)
            { 

                Vector3 pos = Random.insideUnitSphere * duplicateSpawnDistance; //GetRandomPos(6f, transform.position);
                pos += transform.position;

                pos.y = transform.position.y;

                children.Add(Instantiate(duplicatePrefab, pos, Quaternion.identity));

                    
                children[children.Count - 1].GetComponent<EnemyBase>().manager = manager;
                children[children.Count - 1].GetComponent<EnemyBase>().centralHubPos = centralHubPos;
                HasSplit = true;
                
            }
        }
        else
        {
            base.Attack();
        }

    }
}
