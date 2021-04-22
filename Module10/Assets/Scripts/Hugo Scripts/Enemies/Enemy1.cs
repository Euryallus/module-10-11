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
                int tries = 0;

                Vector3 pos = GetRandomPos(6f, transform.position);

                while(pos.y == -300)
                {
                    if(tries < 5)
                    {
                        pos = GetRandomPos(6f, transform.position);
                        tries++;
                    }
                    else
                    {
                        break;
                    }
                }

                if(pos.y != -300)
                {
                    children.Add(Instantiate(duplicatePrefab));

                    children[children.Count - 1].transform.position = pos;
                    children[children.Count - 1].GetComponent<EnemyBase>().manager = manager;
                    HasSplit = true;
                }
            }
        }
        else
        {
            base.Attack();
        }

    }
}
