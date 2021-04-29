using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Anim : Enemy1
{
    public Animator anim;

    protected override void Update()
    {
        base.Update();

        
    }


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
        anim.SetBool("Walk Forward", true);
        anim.SetBool("Run Forward", false);
        base.StartPatrolling();
    }

    public override void StartSearching(Vector3 searchPos)
    {
        base.StartSearching(searchPos);
    }



    public override void Engage()
    {
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Run Forward", true);
        base.Engage();
    }

   public override void Attack()
   {
       if(!HasSplit)
       {
           anim.SetTrigger("Cast Spell");
       }
       else
       {
           if(Random.Range(0, 2) == 0)
           {
               anim.SetTrigger("Stab Attack");
           }
           else
           {
               anim.SetTrigger("Smash Attack");
           }
       }

       base.Attack();
 
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
