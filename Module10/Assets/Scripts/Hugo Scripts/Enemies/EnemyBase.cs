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

    public float stopDistance = 2f;

    [Header("Debug")]
    public float dot;


    private NavMeshAgent agent;
    private GameObject player;

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

        //GoToRandom(15f);

        currentState = EnemyState.stationary;
        player = GameObject.FindGameObjectWithTag("Player");
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
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);

        if (path.status != NavMeshPathStatus.PathPartial)
        {
            agent.SetDestination(targetPosition);
            return true;
        }

        Debug.LogWarning("No path found for " + name);
        return false;
    }

    public virtual void GoToRandom(float maxDistanceFromCurrent)
    {
        Vector3 randomPosition = Random.insideUnitSphere * maxDistanceFromCurrent;

        randomPosition += transform.position;

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, maxDistanceFromCurrent, 1))
        {
            GoTo(hit.position);
        }
        else
        {
            GoToRandom(maxDistanceFromCurrent);
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
        if(CheckForPlayer())
        {
            if (Vector3.Distance(transform.position, player.transform.position) > stopDistance)
            {
                agent.isStopped = false;
                GoTo(player.transform.position);
            }
            else
            {
                agent.isStopped = true;

                TurnTowards(player);
            }
        }
        else
        {
            currentState = EnemyState.stationary;
        }
    }

    public virtual void SearchUpdate()
    {

    }

    public virtual void PatrolUpdate()
    {
        if (Vector3.Distance(transform.position, agent.destination) <= 1f)
        {
            GoToRandom(15f);
        }
    }

    public virtual void EvadeUpdate()
    {

    }

    public virtual bool CheckForPlayer()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

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
}
