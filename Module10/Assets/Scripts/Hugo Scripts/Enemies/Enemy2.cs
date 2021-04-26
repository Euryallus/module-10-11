using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : EnemyBase
{
    public GameObject projectilePrefab;

    private GameObject lastProjectile;

    public Transform projSpawnPoint;

    public float spawnToLaunchTime = 1.5f;

    public override void Start()
    {
        base.Start();

        timeBetweenAttacks += spawnToLaunchTime;
    }

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
        lastProjectile = Instantiate(projectilePrefab);

        lastProjectile.transform.position = projSpawnPoint.position ;

        lastProjectile.transform.parent = gameObject.transform;

        //lastProjectile.GetComponent<Enemy2Projectile>().Launch(dir, player.transform);
        //base.Attack();

        StartCoroutine("Fire");
    }

    private IEnumerator Fire()
    {
        Vector3 dir = player.transform.position - projSpawnPoint.position;
        yield return new WaitForSeconds(spawnToLaunchTime);

        lastProjectile.transform.parent = null;
        lastProjectile.GetComponent<Enemy2Projectile>().Launch(dir, player.transform);
    }
}
