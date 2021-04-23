using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : EnemyBase
{
    public GameObject projectilePrefab;

    private GameObject lastProjectile;


    public override void EngagedUpdate()
    {
        base.EngagedUpdate();

        if(agent.isStopped)
        {
            TurnTowards(player);
        }
    }

    public override void Attack()
    {
        Vector3 dir = player.transform.position - transform.position;

        lastProjectile = Instantiate(projectilePrefab);

        lastProjectile.transform.position = transform.position + transform.forward ;


        lastProjectile.GetComponent<Enemy2Projectile>().Launch(dir, player.transform);
        //base.Attack();
    }
}
