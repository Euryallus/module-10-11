using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1MinionAnim : Enemy1Minion
{
    public Animator anim;


    public override void EngagedUpdate()
    {
        attackCooldown += Time.deltaTime;
        if (CheckForPlayer())
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
            {
                agent.SetDestination(transform.position);
                TurnTowards(player);

                if (attackCooldown > timeBetweenAttacks)
                {


                    Attack();
                    attackCooldown = 0f;
                    return;
                }

                anim.SetBool("Run Forward", false);
            }
            else
            {


                anim.SetBool("Run Forward", true);
                GoTo(playerLastSeen);
            }
        }
        else
        {
            //currentState = EnemyState.search;
            StartSearching(playerLastSeen);
        }
    }

    public override void StartPatrolling()
    {
        base.StartPatrolling();
        anim.SetBool("Walk Forward", true);
        anim.SetBool("Run Forward", false);
    }

    public override void Engage()
    {
        anim.SetBool("Run Forward", true);
        base.Engage();
    }

    public override void Attack()
    {
        base.Attack();

        if (Random.Range(0, 2) == 0)
        {
            anim.SetTrigger("Stab Attack");
        }
        else
        {
            anim.SetTrigger("Smash Attack");
        }
    }

    protected override IEnumerator WaitAndMove(float maxDistance, Vector3 newPointOrigin, float maxWaitTime)
    {
        agent.SetDestination(transform.position);

        anim.SetBool("Walk Forward", false);
        anim.SetBool("Run Forward", false);
        yield return new WaitForSeconds(Random.Range(0f, maxWaitTime));

        findingNewPos = false;
        GoToRandom(maxDistance, newPointOrigin);

        anim.SetBool("Walk Forward", true);
    }
}
