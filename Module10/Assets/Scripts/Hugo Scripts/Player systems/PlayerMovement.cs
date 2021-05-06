using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Main author:         Hugo Bailey
// Additional author:   N/A
// Description:         Handles player movement & movement states 
// Development window:  Prototype phase
// Inherits from:       MonoBehaviour

public class PlayerMovement : MonoBehaviour
{
    [Header("General refs")]

    [SerializeField]    private GameObject playerCamera;    // Reference to player camera (used for forward vect)
    [SerializeField]    private Volume postProcessing;      // Post processing volume (used for water post processing)

    [Header("Player movement speeds")]

    [SerializeField]    [Range(1, 10)]    private float walkSpeed           = 5f;   // Default walk speed
    [SerializeField]    [Range(1, 10)]    private float runSpeed            = 8f;   // Default run speed
    [SerializeField]    [Range(1, 10)]    private float crouchSpeed         = 3f;   // Default crouch speed
    [SerializeField]    [Range(1, 20)]    private float defaultGlideSpeed   = 2f;   // Default glide speed
    [SerializeField]    [Range(1, 10)]    private float swimSpeed           = 5f;   // Default swim speed
    [SerializeField]    [Range(1, 30)]    private float jumpVelocity        = 3f;   // Default jump velocity

    private Dictionary<MovementStates, float> speedMap = new Dictionary<MovementStates, float> { }; // Dictionary mapping movement states to speeds

    [Header("Gravity (normal & glider)")]
    [SerializeField]    [Range(0.5f, 20)]    private float gravity         = 9.81f; // Default downward force

    [Header("Mouse input")]
    [SerializeField]    [Range(0.5f, 8)]    private float mouseSensitivity = 400f;  // Sensitivity of mouse

    [Header("Glider elements")]

    [Tooltip("Rate at which player falls when gliding, default is 9.81 (normal grav.)")]
    [SerializeField]    [Range(0.5f, 12)] private float gliderFallRate                  = 3f;   // Default rate player falls when gliding 
    [SerializeField]    [Range(0.5f, 4)]  private float gliderSensitivity               = 2.0f; // Turn sensitivity of glideer
    [SerializeField]    [Range(0.01f, 1)] private float gliderTiltAmount                = 0.5f; // Amount glider tilts when in use (clamps after [x] amount)
    [SerializeField]    [Range(1, 10)]    private float gliderOpenDistanceFromGround    = 5.0f; // Distance from the ground player must be to open glider


    private float mouseX;           // x component of raw mouse movement
    private float mouseY;           // y component of raw mouse movement
    private float rotateY = 0;      // y Rotation of camera (gets clamped to ~85 degrees to avoid total spinning)
    private float inputX;           // x component of raw keyboard input
    private float inputY;           // y component of raw keyboard input
    private float velocityY = 0;    // Downward velocity acting on playerz

    private CharacterController controller; // Ref to player's character controller

    private DepthOfField dof;   // Ref. to Depth of Field post processing effect
    private Vignette v;         // Ref. to  Vignette post processing effect

    private Vector3 moveTo;         // Vector3 position player moves towards (calculated each frame & passed to CharacterController via Move() func)
    private Vector2 glideVelocity;  // x and z components of glider movement (y comp. is calculated seperately)

    private bool inWater    = false;    // Flags if player is currently underwater
    private bool canMove    = true;     // Flags if player is able to move (changed to prevent moving during dialogue etc.)
    private bool canGlide   = false;    // Flags if player is able to glide (if > [gliderOpenDistanceFromGround] meters off ground)

    private InventoryPanel inventory;   // Ref. to player inventory
    public Item glider;                 // Ref. to glider object

    public enum MovementStates      // Possible states player can be in when moving
    {
        walk,
        run,
        crouch,
        glide,
        dive,
        swim,
        ladder
    }

    private enum CrouchState    // Possible states of "crouching" player can be in
    {
        standing,
        gettingDown,
        crouched,
        gettingUp
    }

    
    private CrouchState currentCrouchState;         // Saves player's current crouch state
    private MovementStates currentMovementState;    // Saves player's current movement state



    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();
        Cursor.lockState = CursorLockMode.Locked;

        currentCrouchState = CrouchState.standing;
        currentMovementState = MovementStates.walk;

        //maps speed float to movement enum
        speedMap[MovementStates.walk] = walkSpeed;
        speedMap[MovementStates.run] = runSpeed;
        speedMap[MovementStates.crouch] = crouchSpeed;
        speedMap[MovementStates.glide] = defaultGlideSpeed;
        speedMap[MovementStates.dive] = swimSpeed;
        speedMap[MovementStates.swim] = swimSpeed;
        speedMap[MovementStates.ladder] = walkSpeed;

        glideVelocity = new Vector2(0, 0);

        //gets post processing effects (water effect)
        postProcessing.profile.TryGet<Vignette>(out v);
        postProcessing.profile.TryGet<DepthOfField>(out dof);
    }

    // Update is called once per frame
    void Update()
    {
        //check if player can glide yet (has obtained glider)


        //raw input from mouse / keyboard (X & Y)
        moveTo = new Vector3(0, 0, 0);

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;// * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;// * Time.deltaTime;

        rotateY -= mouseY;
        rotateY = Mathf.Clamp(rotateY, -75f, 85f);

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        //checks if player is [x] m above ground or not
        if (Physics.Raycast(transform.position, -transform.up, gliderOpenDistanceFromGround))
        {
            canGlide = false;
        }
        else
        {
            canGlide = true;
        }

        //switches state to "run"
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(currentMovementState == MovementStates.walk && controller.isGrounded)
            {
                currentMovementState = MovementStates.run;
            }

        }

        //disables "run"
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            if(currentMovementState == MovementStates.run)
            {
                currentMovementState = MovementStates.walk;
            }
        }

        //toggles crouch
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
                currentMovementState = (currentMovementState == MovementStates.crouch ? MovementStates.walk : MovementStates.crouch);
        }

        //crouch grow / shrink movement
        switch (currentCrouchState)
        {
            //kick starts crouch (movement state is crouch but crouch state is "standing"
            case CrouchState.standing:
                if(currentMovementState == MovementStates.crouch)
                {
                    currentCrouchState = CrouchState.gettingDown;
                }
                break;

                //decreases y scale each update
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

                //checks when player gets back up
            case CrouchState.crouched:
                if(currentMovementState == MovementStates.walk)
                {
                    currentCrouchState = CrouchState.gettingUp;
                }
                break;

                //increases scale on Y each update
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

        //water collision layer
        int mask = 1 << 4;

        if (Physics.Raycast(transform.position - new Vector3(0,0.5f,0), transform.up, 100f, mask ))
        {
            //if hits water layer above & mode isnt swimming, enable post processing effects
            if(currentMovementState != MovementStates.dive)
            {
                dof.active = true;
                v.active = true;
            }

            //flags water bool
            inWater = true;
            //sets current movement mode to diving
            currentMovementState = MovementStates.dive;
        }
        else 
        {
            //if doesnt hit but currently in water
            if (inWater == true)
            {
                //disable post processing effects
                dof.active = false;
                v.active = false;
                //inWater = false;

                //change state to "swim" on top of water
                currentMovementState = MovementStates.swim;
            }
        }

        if(canMove)
        {
            //camera & capsule rotation
            transform.Rotate(Vector3.up * mouseX);
            playerCamera.transform.localRotation = Quaternion.Euler(rotateY, 0, 0f);

            //pos. to move to (by default it's current pos + raw input)
            moveTo = transform.right * inputX + transform.forward * inputY;

            //switch according to movement state
            switch(currentMovementState)
            {
                case MovementStates.swim:
                    //if case = swimming but player is grounded, player is now walking
                    if(controller.isGrounded)
                    {
                        currentMovementState = MovementStates.walk;
                        inWater = false;
                    }

                    //adds gravity 
                    velocityY -= gravity * gravity * Time.deltaTime;
                    
                    //if space is pressed use that as upward velocity rather than cam. forward Y component
                    if(Input.GetKeyDown(KeyCode.Space))
                    {
                        velocityY = jumpVelocity;
                    }

                    break;

                case MovementStates.ladder:

                    moveTo = new Vector3(0, 0, 0);

                    velocityY = inputY * 2f;

                    break;

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

                case MovementStates.dive:

                    moveTo = transform.right * inputX + transform.forward * inputY;
                    velocityY = playerCamera.transform.forward.y * 4f * inputY;

                    if (Input.GetKey(KeyCode.Space))
                    {
                        velocityY = 4f;
                    }
                    break;

                case MovementStates.run:
                case MovementStates.walk:
                case MovementStates.crouch:

                    if (controller.isGrounded)
                    {
                        if (velocityY > 30f)
                        {
                            //velocityY = -0.1f;
                        }
                    }
                    else
                    {
                        velocityY -= gravity * gravity * Time.deltaTime;
                    }

                    break; 
            }         


            if (Input.GetKeyDown(KeyCode.Space) && currentMovementState != MovementStates.crouch && currentMovementState != MovementStates.swim)
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
                else if(currentMovementState == MovementStates.glide)
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

    public void InteractWithLadder()
    {
        currentMovementState = currentMovementState == MovementStates.ladder ? MovementStates.walk : MovementStates.ladder;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentMovementState == MovementStates.ladder)
        {
            currentMovementState = MovementStates.walk;
        }
    }
}
