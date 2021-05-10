using UnityEngine;

// ||=======================================================================||
// || PlayerHeadBobbing: Applies a subtle up/down bobbing effect to the     ||
// ||   player's camera that is adjusted depending on their movement state. ||
// ||=======================================================================||
// || Used on prefab: Player                                                ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHeadBobbing : MonoBehaviour
{
    #region InspectorVariables
    // Variables in this region are set in the inspector

    [SerializeField] private float          bobBaseSpeed     = 13.0f;   // The speed of the bob effect when the player is walking
    [SerializeField] private float          bobBaseIntensity = 0.05f;   // The intensity (up/down movement amount) of the effect when walking
                                                                           
    [SerializeField] private Transform      cameraParentTransform;      // The parent object of the main player camera
    [SerializeField] private PlayerMovement playerMovementScript;       // The script attached to the player that handles movement

    #endregion

    private float time; // Keeps track of time (seconds), used for time-dependent calculations

    void Update()
    {
        float targetYPos; //The camera parent's y position will be set to this value

        if(DoBobbingEffect(out float yBobbingPos))
        {
            //Bobbing effect should be applied, set target to the calculated y bobbing position (see DoBobbingEffect)
            targetYPos = yBobbingPos;
        }
        else
        {
            // Return the target y position back to 0 (default)
            targetYPos = 0.0f;
        }

        // Lerp the camera parent transform's position towards the calculated y value
        cameraParentTransform.localPosition = Vector3.Lerp(cameraParentTransform.localPosition, new Vector3(0.0f, targetYPos, 0.0f), Time.deltaTime * 20.0f);
    }

    private bool DoBobbingEffect(out float yBobbingPos)
    {
        // Check if the player is currently running/crouching/diving
        bool running    = (playerMovementScript.currentMovementState == PlayerMovement.MovementStates.run);
        bool crouching  = (playerMovementScript.currentMovementState == PlayerMovement.MovementStates.crouch);
        bool diving     = (playerMovementScript.currentMovementState == PlayerMovement.MovementStates.dive);

        float bobSpeed      = bobBaseSpeed;     // Use the base bob speed by default
        float bobIntensity  = bobBaseIntensity; // Use the base bob intensity by default
        bool doBobbing      = true;             // Whether the bobbing effect should be applied

        yBobbingPos = 0.0f; //Default y position for if no effect is applied

        if (playerMovementScript.PlayerIsMoving())
        {
            // The player is moving, i.e. not stood still, so a head bobbing effect will be applied

            if (!diving)
            {
                // Player is moving on land, adjust the bobbing speed if the player is running/crouching to make is faster/slower
                if (running)
                {
                    bobSpeed = bobBaseSpeed * 1.5f;
                }
                else if (crouching)
                {
                    bobSpeed = bobBaseSpeed * 0.5f;
                }
            }
            else
            {
                // Player is diving, disable bobbing
                doBobbing = false;
            }
        }
        else
        {
            // Player is stood still

            if (diving)
            {
                // Still in water, slowly bob the camera with a lower intensity
                bobSpeed = bobBaseSpeed * 0.25f;
                bobIntensity = bobBaseIntensity * 0.5f;
            }
            else
            {
                // Still on land, disable bobbing
                doBobbing = false;
            }
        }

        if(doBobbing)
        {
            // Calculate the y position of the camera to apply the bobbing effect (used if doBobbing = true)
            yBobbingPos = GetTargetYBobbingPos(bobSpeed, bobIntensity);
        }

        return doBobbing;
    }

    private float GetTargetYBobbingPos(float bobSpeed, float intensity)
    {
        // Increment the timer used for sin calculation
        time += (Time.deltaTime * bobSpeed);

        // Set the target y position using a sin function based on the bob intensity to give a smooth up/down movement over time
        return (Mathf.Sin(time) * intensity) + intensity;
    }
}