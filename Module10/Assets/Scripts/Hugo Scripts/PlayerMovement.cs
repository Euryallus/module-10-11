using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("THIS NEEDS IMPROVING SOME TIME")]
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private CharacterController controller;

    private float mouseX;
    private float mouseY;
    private float rotateY = 0;
    private float inputX;
    private float inputY;

    private float velocityY = 0;

    [Header("Speeds")]
    [SerializeField]
    [Range(1, 10)]
    private float walkSpeed = 5;
    [SerializeField]
    [Range(1, 10)]
    private float runSpeed = 8;
    [SerializeField]
    [Range(1, 10)]
    private float crouchSpeed = 3;
    [SerializeField]
    [Range(1, 20)]
    private float defaultGlideSpeed = 2f;
    [SerializeField]
    [Range(1, 10)]
    private float swimSpeed= 5f;
    [SerializeField]
    [Range(1, 30)]
    private float jumpVelocity = 3f;

    [Header("Gravity")]
    [SerializeField] 
    [Tooltip("Rate at which player falls when gliding, default is 9.81 (normal grav.)")]
    [Range(0.5f, 12)]
    private float gliderFallRate = 3f;
    [SerializeField]
    [Range(0.5f, 20)]
    private float gravity = 9.81f;

    private Vector3 moveTo;

    [Header("Mouse input")]
    [SerializeField]
    [Range(0.5f, 8)]
    private float mouseSensitivity = 400f;

    [Header("Glider stuff")]
    [SerializeField]
    [Range(0.5f, 4)]
    private float gliderSensitivity = 2.0f;

    [SerializeField]
    [Range(0.01f, 1)]
    private float gliderTiltAmount = 0.5f;
    
    [SerializeField]
    [Range(1, 10)]
    private float gliderOpenDistanceFromGround = 5.0f;

    private bool jumpForceAdded = false;

    private bool canMove = true;

    private bool canGlide = false;

    private Vector2 glideVelocity;

    public enum MovementStates
    {
        walk,
        run,
        crouch,
        glide,
        swim
    }

    private enum CrouchState
    {
        standing,
        gettingDown,
        crouched,
        gettingUp
    }

    private CrouchState currentCrouchState;
    public MovementStates currentMovementState;

    private Dictionary<MovementStates, float> speedMap = new Dictionary<MovementStates, float> { };


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currentCrouchState = CrouchState.standing;
        currentMovementState = MovementStates.walk;
        speedMap[MovementStates.walk] = walkSpeed;
        speedMap[MovementStates.run] = runSpeed;
        speedMap[MovementStates.crouch] = crouchSpeed;
        speedMap[MovementStates.glide] = defaultGlideSpeed;
        speedMap[MovementStates.swim] = swimSpeed;

        glideVelocity = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        moveTo = new Vector3(0, 0, 0);

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;// * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;// * Time.deltaTime;

        rotateY -= mouseY;
        rotateY = Mathf.Clamp(rotateY, -75f, 75f);

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if (Physics.Raycast(transform.position, -transform.up, gliderOpenDistanceFromGround))
        {
            canGlide = false;
        }
        else
        {
            canGlide = true;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(currentMovementState == MovementStates.walk && controller.isGrounded)
            {
                currentMovementState =MovementStates.run;
            }

        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            if(currentMovementState == MovementStates.run)
            {
                currentMovementState = MovementStates.walk;
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            currentMovementState = (currentMovementState == MovementStates.crouch ? MovementStates.walk : MovementStates.crouch);
        }

        switch (currentCrouchState)
        {
            case CrouchState.standing:
                if(currentMovementState == MovementStates.crouch)
                {
                    currentCrouchState = CrouchState.gettingDown;
                }
                break;

            case CrouchState.gettingDown:
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(1, 0.6f, 1), 3 * Time.deltaTime);

                if(gameObject.transform.localScale.y - 0.6f <= 0.01f)
                {
                    gameObject.transform.localScale = new Vector3(1, 0.6f, 1);
                    currentCrouchState = CrouchState.crouched;
                }

                if (currentMovementState == MovementStates.walk)
                {
                    currentCrouchState = CrouchState.gettingUp;
                }

                break;

            case CrouchState.crouched:
                if(currentMovementState == MovementStates.walk)
                {
                    currentCrouchState = CrouchState.gettingUp;
                }
                break;

            case CrouchState.gettingUp:

                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.one, 3 * Time.deltaTime);

                if (1 - gameObject.transform.localScale.y <= 0.01f)
                {
                    gameObject.transform.localScale = Vector3.one;
                    currentCrouchState = CrouchState.standing;
                }
                break;
            default:
                break;
        }

        if(canMove)
        {
            transform.Rotate(Vector3.up * mouseX);
            playerCamera.transform.localRotation = Quaternion.Euler(rotateY, 0, 0f);
            moveTo = transform.right * inputX + transform.forward * inputY;

            switch(currentMovementState)
            {
                case MovementStates.glide:
                    if (controller.isGrounded)
                    {
                        currentMovementState = MovementStates.walk;
                        glideVelocity = new Vector2(0, 0);
                    }

                    if (inputY == 0)
                    {
                        glideVelocity.y -= glideVelocity.y * 0.25f;
                    }
                    //if y input, increase forward velocity
                    else
                    {
                        glideVelocity.y += inputY * (gliderSensitivity / 2) * Time.deltaTime;
                    }

                    if (Physics.Raycast(transform.position, -transform.up, gliderOpenDistanceFromGround * 2))
                    {
                        if (glideVelocity.x > 0.01f || glideVelocity.x < -0.01f)
                        {
                            if (glideVelocity.x < 0)
                            {
                                glideVelocity.x += 3 * Time.deltaTime;
                            }
                            else
                            {
                                glideVelocity.x -= 3 * Time.deltaTime;
                            }
                        }

                    }
                    else
                    {
                        if (inputX == 0)
                        {
                            if (glideVelocity.x > 0.01f || glideVelocity.x < -0.01f)
                            {
                                if (glideVelocity.x < 0)
                                {
                                    glideVelocity.x += 3 * Time.deltaTime;
                                }
                                else
                                {
                                    glideVelocity.x -= 3 * Time.deltaTime;
                                }
                            }
                        }
                        else
                        {
                            glideVelocity.x += inputX * gliderSensitivity * Time.deltaTime;
                        }
                    }

                    Quaternion target = playerCamera.transform.localRotation;
                    target.z = Mathf.Deg2Rad * -glideVelocity.x;

                    if (target.z < gliderTiltAmount && target.z > -gliderTiltAmount)
                    {
                        playerCamera.transform.localRotation = target;
                        Debug.Log(target);
                    }
                    else
                    {
                        if (target.z < 0)
                        {
                            target.z = -gliderTiltAmount;

                        }
                        else
                        {
                            target.z = gliderTiltAmount;
                        }

                        playerCamera.transform.localRotation = target;
                    }

                    glideVelocity.x = Mathf.Clamp(glideVelocity.x, -speedMap[currentMovementState] / 2, speedMap[currentMovementState] / 2);

                    glideVelocity.y = Mathf.Clamp(glideVelocity.y, -0.2f, speedMap[currentMovementState]);

                    moveTo = transform.right * glideVelocity.x + transform.forward * glideVelocity.y;

                    velocityY -= gliderFallRate * gliderFallRate * Time.deltaTime;

                    break;

                case MovementStates.swim:
                    moveTo = transform.right * inputX + transform.forward * inputY;

                    velocityY = playerCamera.transform.forward.y * 4f * inputY;

                    break;

                case MovementStates.run:
                case MovementStates.walk:
                case MovementStates.crouch:

                    if (controller.isGrounded)
                    {
                        if (velocityY > 30f)
                        {
                            velocityY = -0.1f;
                        }
                    }
                    else
                    {
                        velocityY -= gravity * gravity * Time.deltaTime;
                    }

                    break;

                
            }         

            if (Input.GetKeyDown(KeyCode.Space) && currentMovementState != MovementStates.crouch)
            {
                if (controller.isGrounded)
                {
                    velocityY = jumpVelocity;
                }
                else if (currentMovementState != MovementStates.glide && canGlide)
                {
                    currentMovementState = MovementStates.glide;

                    glideVelocity = new Vector2(inputX, inputY).normalized;

                    velocityY = 0.1f;
                }
                else
                {
                    currentMovementState = MovementStates.walk;
                }
            }

            if (moveTo.magnitude > 1)
            {
                moveTo.Normalize();
            }

            Vector3 moveVect = moveTo * speedMap[currentMovementState];

            moveVect.y = velocityY;

            controller.Move(moveVect * Time.deltaTime); //applies movement to player
        }
    }

    public bool PlayerIsGrounded()
    {
        return controller.isGrounded;
    }

    public void SetJumpVelocity(float velocity)
    {
        if(controller.isGrounded)
        {
            velocityY = velocity;
        }
    }
    public void StopMoving()
    {
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public bool PlayerIsMoving()
    {
        if(canMove && (Mathf.Abs(inputX) > 0.01f || Mathf.Abs(inputY) > 0.01f))
        {
            return true;
        }
        return false;
    }

    public MovementStates GetCurrentMovementState()
    {
        return currentMovementState;
    }
}
