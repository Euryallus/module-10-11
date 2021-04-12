using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHeadBobbing : MonoBehaviour
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private float          bobBaseSpeed     = 13.0f;   //The speed of the bob effect when the player is walking
    [SerializeField] private float          bobBaseIntensity = 0.05f;   //The intensity (up/down movement amount) of the effect when walking

    [SerializeField] private Transform      cameraParentTransform;      //The parent object of the main player camera
    [SerializeField] private PlayerMovement playerMovementScript;       //The script attached to the player that handles movement

    #endregion

    private float time; //Keeps track of time (seconds), used for time-dependent calculations

    void Update()
    {
        float targetYPos;   //The camera parent's y position will be set to this value

        if (playerMovementScript.PlayerIsMoving())
        {
            //The player is moving, i.e. not stood still, so a head bobbing effect will be applied

            //Check if the player is currently running or crouching
            bool running    = (playerMovementScript.currentMovementState == PlayerMovement.MovementStates.run);
            bool crouching  = (playerMovementScript.currentMovementState == PlayerMovement.MovementStates.crouch);

            //Get the default speed for the bobbing effect
            float bobSpeed = bobBaseSpeed;

            //Adjust this speed if the player is running/crouching to make is faster/slower
            if (running)
            {
                bobSpeed *= 1.5f;
            }
            else if (crouching)
            {
                bobSpeed *= 0.5f;
            }

            //Increment the timer used for sin calculation
            time += (Time.deltaTime * bobSpeed);

            //Set the target y position using a sin function based on the bob intensity to give a smooth up/down movement over time
            targetYPos = (Mathf.Sin(time) * bobBaseIntensity) + bobBaseIntensity;

        }
        else
        {
            //The player is stood still
            //Smoothly lerp the target y position back to 0

            targetYPos = Mathf.Lerp(cameraParentTransform.localPosition.y, 0.0f, Time.deltaTime * bobBaseSpeed);
        }

        //Set the camera parent transform's position based on the calculated y value
        cameraParentTransform.localPosition = new Vector3(0.0f, targetYPos, 0.0f);
    }
}