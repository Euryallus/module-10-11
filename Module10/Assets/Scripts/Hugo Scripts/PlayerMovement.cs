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

    [SerializeField]
    private float walkSpeed = 5;
    [SerializeField]
    private float runSpeed = 8;
    [SerializeField]
    private float crouchSpeed = 3;

    private Vector3 moveTo;

    [SerializeField]
    private float mouseSensitivity = 400f;

    [SerializeField]
    private float jumpVelocity = 100f;

    private bool canMove = true;

    public enum MovementStates
    {
        walk,
        run,
        crouch,
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

        if(Input.GetKeyDown(KeyCode.Space) && currentMovementState != MovementStates.crouch)
        {
            if(controller.isGrounded)
            {
                velocityY = jumpVelocity;
            }

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

            if(!controller.isGrounded)
            {
                velocityY -= 9.81f * Time.deltaTime;
                moveTo.y = velocityY;
            }

            controller.Move(moveTo * speedMap[currentMovementState] * Time.deltaTime); //applies movement to player
        }
    }

    public bool PlayerIsGrounded()
    {
        return controller.isGrounded;
    }

    public void SetJumpVelocity(float velocity)
    {
        velocityY = velocity;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
    }
}
