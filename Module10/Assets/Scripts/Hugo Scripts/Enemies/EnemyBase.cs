using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    private NavMeshAgent agent;

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

        GoToRandom(15f);

        currentState = EnemyState.patrol;
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

    }

    public virtual void EngagedUpdate()
    {

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
}
