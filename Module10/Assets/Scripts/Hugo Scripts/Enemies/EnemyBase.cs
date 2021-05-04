using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy properties")]
    public float viewDistance = 25f;


    [Range(1.0f, 360f)]
    public float viewAngle = 120f;
    public float stationaryTurnSpeed = 5f;

    public int difficulty = 1;

    public Vector3 playerLastSeen;
    public int noOfSearchPoints = 8;
    private int searchPointsVisited = 0;

    public float patrolWanderDistance = 15f;
    public float searchDiameter = 10f;
    public Vector3 centralHubPos;


    [Header("Combat stuff (Ensure attack distance is > stopping distance!!)")]
    public float baseDamage;
    public float timeBetweenAttacks = 2f;
    protected float attackCooldown;
    public float attackDistance;

    private float dot;
    protected NavMeshAgent agent;
    protected GameObject player;
    protected PlayerStats playerStats;

    private float distToPlayer;
    public EnemyCampManager manager;

    [Header("Behaviours")]
    [Range(0f, 10f)]
    public float maxAtEachPatrolPoint;
    protected bool findingNewPos = false;
    public float patrolSpeed;
    private float defaultSpeed;

    public enum EnemyState
    {
        stationary,
        engaged,
        search,
        patrol,
        evade
    }

    public EnemyState currentState;


    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();

        defaultSpeed = agent.speed;

        StartPatrolling();
    }

    // Update is called once per frame
    protected virtual void Update()
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

    //goes to location passed on navmesh
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

    //goes to random position [x] meters away from the origin
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

    //stationary state update
    public virtual void StationaryUpdate()
    {
        if(CheckForPlayer())
        {
            currentState = EnemyState.engaged;
        }
    }

    //engaged state update
    public virtual void EngagedUpdate()
    {
        //increases time since last attack
        attackCooldown += Time.deltaTime;

        //if player is spotted, act accordingly
        if (CheckForPlayer())
        {
            //check distance between player & enemy
            if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
            {
                //if can see & player is within attack distance, stop moving & turn to face player
                agent.SetDestination(transform.position);
                TurnTowards(player);

                //once cooldown is over, attack player & reset cooldown
                if (attackCooldown > timeBetweenAttacks)
                {

                    Attack();
                    attackCooldown = 0f;
                    return;
                }
            }
            else
            {
                //if player is visible but not reachable, follow them
                GoTo(playerLastSeen);
            }
        }
        else
        {
            //if player has been lost while engaged, switch to searching the area
            StartSearching(playerLastSeen);
        }
    }

    //search state update
    public virtual void SearchUpdate()
    {
        //if player is seen while searching, try to alert other units
        if (CheckForPlayer())
        {
            if (manager != null)
            {
                manager.AlertUnits(playerLastSeen);
            }

            //switch state to "engaged" and start following player

            Engage();

            //currentState = EnemyState.engaged;
            return;
        }

        //if player hasnt been spotted and player is within ~3m of the destination stored, run "wait and move on" co-routine 
        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance * 1.5f && !findingNewPos)
        {
            //search points incrimented (prevents searching forever)
            ++searchPointsVisited;

            if(searchPointsVisited < noOfSearchPoints)
            {
                
                    StartCoroutine(WaitAndMove(searchDiameter, playerLastSeen, 0.5f));
                    findingNewPos = true;
                
                
                //GoToRandom(searchDiameter, playerLastSeen);

            }
            else
            {
                //if enemy has visited enough search points, stop searching & start patrolling
                StartPatrolling();
            }
        }
    }

    //patrol state update
    public virtual void PatrolUpdate()
    {
        //if player is spotted, switch to engaged state & return
        if(CheckForPlayer())
        {

            Engage();

            return;
        }

        //if player isn't seen & enemy is within ~ 3m of destination, pick a new point to go to centred on the central hub position
        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance * 1.5f && !findingNewPos)
        {
            StartCoroutine(WaitAndMove(patrolWanderDistance, centralHubPos, maxAtEachPatrolPoint));
            //bool flagged to prevent coroutine being run 4000 times
            findingNewPos = true;
            
        }
    }

    //evate state update
    public virtual void EvadeUpdate()
    {

    }

    //begins searching behaviour
    public virtual void StartSearching(Vector3 searchPos)
    {
        //sets player last seen position to pos. passed as parameter
        playerLastSeen = searchPos;
        //sets first search destination to be that position
        GoTo(playerLastSeen);
        //resets points count to 0
        searchPointsVisited = 0;
        //sets state to "search"
        currentState = EnemyState.search;

        //resets agents speed to "max"
        agent.speed = defaultSpeed;

        //debug help
        Debug.Log(gameObject.name + " started searching " + searchPos);
    }

    //begins patrol behaviour
    public virtual void StartPatrolling()
    {
        //searchPointsVisited = 0;
        //sets agent speed to patrol speed (slower than engaged speed"
        agent.speed = patrolSpeed;

        //stops any co-routine left from search func.
        StopCoroutine("WaitAndMove");

        //calls co-routine to move in [x] seconds to random pos. centred around central hub
        WaitAndMove(patrolWanderDistance, centralHubPos, 1.0f);
        //sets current state to "patrol"
        currentState = EnemyState.patrol;
    }

    //checks if player is visible using view angle specified and view distance
    public virtual bool CheckForPlayer()
    {
        //stores distance from enemy to player
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //if enemy can "see" player based on distance, continue evaluating
        if(distToPlayer <= viewDistance)
        {
            //calculates dot product between enemy forward vect. and player position
            dot = Vector3.Dot(transform.transform.forward.normalized, (player.transform.position - transform.position).normalized);
            //calculates angle from dot product
            dot = Mathf.Acos(dot);

            //if angle calculated is < view angle defined, player is within view cone
            if (dot <= Mathf.Deg2Rad * viewAngle / 2 )
            {
                //creates raycast mask for excluding colliders on "enemies" layer
                int mask = 1 << 6;

                //raycasts from enemy towards player - if it hits, nothing's obstructing the view
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, viewDistance, ~mask))
                {
                    //if raycast does hit player
                    if (hit.transform.CompareTag("Player"))
                    {
                        //draw a debug line to show connection, store position player was seen at, and return true
                        Debug.DrawLine(transform.position, hit.transform.position, Color.red);
                        playerLastSeen = player.transform.position;
                        searchPointsVisited = 0;
                        return true;
                    }
                }
            }
        }

        //if player is not visible, return false
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

    //base attack - when within range, raycasts towards player, & if it hits player loses health (only works for melee)
    public virtual void Attack()
    {
        //raycasts from enemy to player - if it hits, the player can be hurt
        if(Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, attackDistance))
        {
            //debug assist
            Debug.LogWarning("Hit " + hit.transform.name);

            //if hit player & the player has health script, do damage
            if(hit.transform.CompareTag("Player"))
            {
                if(playerStats!= null)
                {
                    playerStats.DecreaseHealth(baseDamage);
                }

            }
        }
    }

    //checks if point passed is on navmesh
    public bool IsPointOnNavMesh(Vector3 pos, float maxDist)
    {
        return NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDist, 1);
    }

    //gets a random position definately on the navmesh centred around origin (not currently used - very costly)
    public Vector3 GetRandomPos(float maxDistanceFromCurrent, Vector3 origin)
    {
        Vector3 randomPosition = Random.insideUnitSphere.normalized * Random.Range(3, maxDistanceFromCurrent);

        randomPosition += origin;

        NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, maxDistanceFromCurrent, 1);
        return hit.position;

    }

    //used by the enemy camp to signal that the enemy has been spotted
    public void AlertOfPosition(Vector3 position)
    {
        //checks if player is already engaged with player
        if(currentState != EnemyState.engaged)
        {
            //if not, update player last seen and start searching the area
            playerLastSeen = position;

            StartSearching(position);
        }

    }

    //called when enemy enters engaged state - resets speed, stops any co-routines, and sets state to engaged
    public virtual void Engage()
    {
        agent.speed = defaultSpeed;
        StopCoroutine("WaitAndMove");
        currentState = EnemyState.engaged;
    }

    //used to wait for random amount of time at a position before moving on, aims to make enemies seem more "natural"
    protected virtual IEnumerator WaitAndMove(float maxDistance, Vector3 newPointOrigin, float maxWaitTime)
    {
        //stops agent from moving
        agent.SetDestination(transform.position);

        //waits for x time 
        yield return new WaitForSeconds(Random.Range(0f, maxWaitTime));
        //flags bool as false now waitForSeconds is over
        findingNewPos = false;
        //tells agent to go to random position around origin 
        GoToRandom(maxDistance, newPointOrigin);
    }
}
