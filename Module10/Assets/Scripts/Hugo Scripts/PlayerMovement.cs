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

    [SerializeField]
    private float velocityY = 0;

    [SerializeField]
    private float walkSpeed = 5;
    [SerializeField]
    private float runSpeed = 8;
    [SerializeField]
    private float crouchSpeed = 3;
    [SerializeField]
    private float defaultGlideSpeed = 2f;
    [SerializeField]
    [Tooltip("Rate at which player falls when gliding, default is 9.81 (normal grav.)")]
    private float gliderFallRate = 3f;
    [SerializeField]
    private float gravity = 9.81f;

    private Vector3 moveTo;

    [SerializeField]
    private float mouseSensitivity = 400f;

    [SerializeField]
    private float jumpVelocity = 3f;

    private bool jumpForceAdded = false;

    private bool canMove = true;

    private Vector2 glideVelocity;

    public enum MovementStates
    {
        walk,
        run,
        crouch,
        glide
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

        glideVelocity = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
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
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotateY -= mouseY;
            rotateY = Mathf.Clamp(rotateY, -75f, 75f);

            transform.Rotate(Vector3.up * mouseX);
            playerCamera.transform.localRotation = Quaternion.Euler(rotateY, 0, 0f);

            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");

            moveTo = transform.right * inputX + transform.forward * inputY;
            moveTo.Normalize();


            if(controller.isGrounded && currentMovementState == MovementStates.glide)
            {
                currentMovementState = MovementStates.walk;
                glideVelocity = new Vector2(0, 0);
            }

            if (controller.isGrounded)
            {
                if(velocityY > 30f)
                {
                    velocityY = -0.1f;
                }

            }
            else if(currentMovementState != MovementStates.glide)
            {
                velocityY -= gravity * gravity * Time.deltaTime;
            }            

            if (Input.GetKeyDown(KeyCode.Space) && currentMovementState != MovementStates.crouch)
            {
                if (controller.isGrounded)
                {
                    velocityY = jumpVelocity;
                }
                else if (currentMovementState != MovementStates.glide)
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

            if (currentMovementState == MovementStates.glide)
            {
                //cumilative r / l velocity
                // w increases speed, increases downward velocity
                // s decreases speed, decreases downward velocity
                // rotate l or r based on velocity

                glideVelocity.x += inputX * 2f * Time.deltaTime;
                glideVelocity.y += inputY * 1f * Time.deltaTime;

                glideVelocity.x = Mathf.Clamp(glideVelocity.x, -2.0f, 2.0f);
                glideVelocity.y = Mathf.Clamp(glideVelocity.y, -0.3f, 1.0f);

                moveTo = transform.right * glideVelocity.x + transform.forward * glideVelocity.y;
                moveTo.Normalize();

                //moveTo.Normalize();

                float vert = glideVelocity.y * 1.3f;
                vert = Mathf.Clamp(vert, 0.3f, 1.3f);

                velocityY -= (gliderFallRate * vert) * (gliderFallRate * vert) * Time.deltaTime;

                //velocityY -= gliderFallRate * gliderFallRate * Time.deltaTime;
            }


            Vector3 moveVect = moveTo * speedMap[currentMovementState];

            moveVect.y = velocityY;

            Debug.Log(moveVect);

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
