using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy properties")]
    public float viewDistance = 25f;
    public float viewAngle = 120f;
    public float stationaryTurnSpeed = 5f;

    public int difficulty = 1;

    public float stopDistance = 2f;

    public Vector3 playerLastSeen;
    private int searchPointsVisited = 0;

    public Vector3 centralHubPos;

    [Header("Combat stuff")]
    public float baseDamage;
    public float timeBetweenAttacks = 2f;
    private float attackCooldown;
    public float attackDistance;

    private float dot;
    protected NavMeshAgent agent;
    protected GameObject player;
    private PlayerStats playerStats;

    private float distToPlayer;

    public bool canReach = true;

    public EnemyCampManager manager;

    public Vector3 enemyDestination;

    public enum EnemyState
    {
        stationary,
        engaged,
        search,
        patrol,
        evade
    }

    public EnemyState currentState;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentState = EnemyState.search;
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        
        GoToRandom(20f, centralHubPos);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.stationary:

                StationaryUpdate();

                break;

            case EnemyState.engaged:

                EngagedUpdate();

                break;

            case EnemyState.search:

                SearchUpdate();

                break;

            case EnemyState.patrol:

                PatrolUpdate();

                break;

            case EnemyState.evade:

                EvadeUpdate();

                break;

            default:

                Debug.LogWarning("Something's wrong with " + name);

                break;
        }

    }

    public virtual bool GoTo(Vector3 targetPosition)
    {   //
        //NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5f, 1);
        //if (hit.position.x != Mathf.Infinity)
        //{
        //    NavMeshPath path = new NavMeshPath();
        //    agent.CalculatePath(targetPosition, path);
        //
        //    if (path.status != NavMeshPathStatus.PathPartial)
        //    {
        //        enemyDestination = targetPosition;
        //        
        //        return true;
        //    }
        //}
        agent.SetDestination(targetPosition);
        //Debug.LogWarning("No path found for " + name);
        return true;
    }

    public virtual void GoToRandom(float maxDistanceFromCurrent, Vector3 origin)
    {
        Vector3 randomPosition = Random.insideUnitSphere * maxDistanceFromCurrent;

        //randomPosition.y = transform.position.y;

        randomPosition += origin;

        if(!GoTo(randomPosition))
        {
            GoTo(centralHubPos);
        }
    }

    public virtual void StationaryUpdate()
    {
        if(CheckForPlayer())
        {
            currentState = EnemyState.engaged;
        }
    }

    public virtual void EngagedUpdate()
    {
        attackCooldown += Time.deltaTime;
        if(CheckForPlayer())
        {
            if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
            {
                agent.isStopped = false;

                if(!GoTo(player.transform.position))
                {
                    if(canReach == true)
                    {
                        canReach = false;

                        GoToRandom(15f, playerLastSeen);
                    }
                    else
                    {
                        if(agent.isStopped)
                        {
                            GoToRandom(15f, playerLastSeen);
                        }
                    }

                }
                else
                {
                    canReach = true;
                }
            }
            else
            {
                agent.isStopped = true;
                TurnTowards(player);

                if(attackCooldown > timeBetweenAttacks)
                {
                    attackCooldown = 0f;
                    Attack();
                }
            }
        }
        else
        {
            StartSearching(playerLastSeen);
        }
    }

    public virtual void SearchUpdate()
    {
        if (CheckForPlayer())
        {
            manager.AlertUnits(playerLastSeen);
            currentState = EnemyState.engaged;
            return;
        }

        if (Vector3.Distance(transform.position, agent.destination) < 2.5f)
        {
            ++searchPointsVisited;

            if(searchPointsVisited < 5)
            {
                GoToRandom(25f, playerLastSeen);

            }
            else
            {
                StartPatrolling();
            }
        }
    }

    public virtual void PatrolUpdate()
    {

        if(CheckForPlayer())
        {
            //manager.AlertUnits(playerLastSeen);
            
            currentState = EnemyState.engaged;
            return;
        }

        if (Vector3.Distance(transform.position, agent.destination) <= 3f)
        {
            GoToRandom(35f, centralHubPos);
        }
    }

    public virtual void EvadeUpdate()
    {

    }

    public virtual void StartSearching(Vector3 searchPos)
    {
        //playerLastSeen = searchPos;
        GoToRandom(7f, playerLastSeen);
        searchPointsVisited = 0;
        currentState = EnemyState.search;
        //Debug.Log(gameObject.name + " started searching " + searchPos);
    }

    public virtual void StartPatrolling()
    {
        searchPointsVisited = 0;
        GoToRandom(40f, centralHubPos);
        currentState = EnemyState.patrol;
    }

    public virtual bool CheckForPlayer()
    {
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if(distToPlayer <= viewDistance)
        {
            dot = Vector3.Dot(transform.transform.forward.normalized, (player.transform.position - transform.position).normalized);
            dot = Mathf.Acos(dot);

            if (dot <= Mathf.Deg2Rad * viewAngle / 2 )
            {
                int mask = 1 << 6;

                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, viewDistance, ~mask))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.DrawLine(transform.position, hit.transform.position, Color.red);
                        playerLastSeen = player.transform.position;
                        searchPointsVisited = 0;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public virtual void TurnTowards(GameObject target)
    {
        //https://answers.unity.com/questions/351899/rotation-lerp.html

        Vector3 lookTarget = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
        lookTarget.x = 0;
        lookTarget.z = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(lookTarget), stationaryTurnSpeed * Time.deltaTime);

        //transform.LookAt(target.transform);
    }

    public virtual void Attack()
    {
        if(Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, attackDistance))
        {
            if(hit.transform.CompareTag("Player"))
            {
                if(playerStats!= null)
                {
                    //playerStats.DecreaseHealth(baseDamage);
                }

            }
        }
    }

    public bool IsPointOnNavMesh(Vector3 pos, float maxDist)
    {
        return NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDist, 1);
    }

    public Vector3 GetRandomPos(float maxDistanceFromCurrent, Vector3 origin)
    {
        Vector3 randomPosition = Random.insideUnitSphere.normalized * Random.Range(3, maxDistanceFromCurrent);

        randomPosition += origin;

        NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, maxDistanceFromCurrent, 1);
        return hit.position;

    }

    public void AlertOfPosition(Vector3 position)
    {
        if(currentState != EnemyState.engaged)
        {
            playerLastSeen = position;

            StartSearching(position);
        }

    }
}
