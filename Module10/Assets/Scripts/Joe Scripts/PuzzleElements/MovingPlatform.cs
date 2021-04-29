using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PlatformButtonBehaviour defines how moving platforms react to
//  button presses/releases when connected to a PuzzleButton
public enum PlatformButtonBehaviour
{
    TriggerOnPress, //Starts moving when the button is pressed, returns to start point when released
    PauseOnPress    //Moves as normal by default but pauses in place while the button is pressed
}

//PlatformMovementType defines the movement pattern of a platform
public enum PlatformMovementType
{
    OutAndBack,     //The platform will move through all points from first to last, then return to the start
    Loop            //The platform will move through all points from first to last, then instantly start again from the first
}

public enum PlatformMoveDirection
{
    Forwards,
    Backwards,
    None
}

public class MovingPlatform : MonoBehaviour
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [Space]
    [Header("(See tooltips for info)")]
    [Header("Moving Platform")]

    [SerializeField] [Tooltip("The points that form the movement path of the platform. Note: these points are relative to the GameObject's position in the world")]
    private Vector3[]            movePoints = new Vector3[] { Vector3.zero, new Vector3(10.0f, 0.0f, 0.0f) };

    [SerializeField] [Tooltip("OutAndBack: Move through all points from first to last, then return back in the opposite direction\n\n" +
                                "Loop: Move through all points from first to last, then instantly start again from the first\n\n"+
                                "NOTE: Movement type is ignored if connected to a button using TriggerOnPress behaviour")]
    private PlatformMovementType movementType;

    [SerializeField] [Tooltip("How quickly the platform moves between points")]
    private float                moveSpeed = 6.0f;

    #endregion

    #region Properties

    public bool Paused              { set { paused = value; } }
    public bool TriggeredByButton   { set { triggeredByButton = value; } }

    #endregion

    private int                     currentPointIndex;
    private Vector3                 basePosition;
    private bool                    paused;
    private PlatformMoveDirection   moveDirection;
    private bool                    triggeredByButton;

    private Transform               playerReturnToTransform;

    void Start()
    {
        basePosition = transform.position;

        if (!triggeredByButton)
        {
            moveDirection = PlatformMoveDirection.Forwards;
        }
        else
        {
            moveDirection = PlatformMoveDirection.None;
        }
    }

    void Update()
    {
        float distanceToMove = moveSpeed * Time.deltaTime;

        if(!paused && moveDirection != PlatformMoveDirection.None)
        {
            if (Vector3.Distance(transform.position, basePosition + movePoints[currentPointIndex]) <= distanceToMove)
            {
                FindNextPoint();
            }

            transform.position = Vector3.MoveTowards(transform.position, basePosition + movePoints[currentPointIndex], distanceToMove);
        }
        else
        {
           //Paused or not moving
        }
    }

    private void FindNextPoint()
    {
        if (moveDirection == PlatformMoveDirection.Forwards)
        {
            if (currentPointIndex < (movePoints.Length - 1))
            {
                currentPointIndex++;
            }
            else
            {
                if(triggeredByButton)
                {
                    moveDirection = PlatformMoveDirection.None;
                }
                else
                {
                    if (movementType == PlatformMovementType.OutAndBack)
                    {
                        moveDirection = PlatformMoveDirection.Backwards; //Reached end, go back
                    }
                    else //movementType == Loop
                    {
                        currentPointIndex = 0;  //Reached end, loop back to start
                    }
                }
            }
        }
        else if(moveDirection == PlatformMoveDirection.Backwards)
        {
            if (currentPointIndex > 0)
            {
                currentPointIndex--;
            }
            else
            {
                if(triggeredByButton)
                {
                    moveDirection = PlatformMoveDirection.None;
                }
                else
                {
                    moveDirection = PlatformMoveDirection.Forwards; //Reached start, begin moving forwards again
                }
            }
        }
    }

    public void StartMovingForwards()
    {
        moveDirection = PlatformMoveDirection.Forwards;
        FindNextPoint();
    }

    public void StartMovingBackwards()
    {
        moveDirection = PlatformMoveDirection.Backwards;
        FindNextPoint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Physics.autoSyncTransforms = true;

            playerReturnToTransform = other.transform.parent;

            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.autoSyncTransforms = false;

        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(playerReturnToTransform);
        }
    }

    private void OnDrawGizmos()
    {
        if(movePoints != null && movePoints.Length > 0)
        {
            for (int i = 0; i < movePoints.Length; i++)
            {
                Vector3 worldPos = transform.position + movePoints[i];

                //Line connecting the current and next points
                if (i < (movePoints.Length - 1))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(worldPos, transform.position + movePoints[i + 1]);
                }

                Gizmos.color = Color.red;

                //Line to show downwards direction
                Gizmos.DrawLine(worldPos, worldPos - new Vector3(0.0f, 1.0f, 0.0f));

                //Sphere showing point position
                Gizmos.DrawSphere(worldPos, 0.5f);
            }
        }
    }
}
