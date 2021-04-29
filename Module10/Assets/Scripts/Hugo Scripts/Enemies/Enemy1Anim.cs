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

                anim.SetBool("Walk Forward", false);
            }
            else
            {


                anim.SetBool("Walk Forward", true);
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
        base.StartPatrolling();
    }

    public override void StartSearching(Vector3 searchPos)
    {
        base.StartSearching(searchPos);
        anim.SetBool("Walk Forward", true);
    }



    public override void Engage()
    {
        anim.SetBool("Walk Forward", true);
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
}
