using UnityEngine;

// ||=======================================================================||
// || PlayerFootsteps: Plays footstep sounds at a regular interval          ||
// ||    depending on how quickly the player is moving.                     ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

[RequireComponent(typeof(PlayerMovement))]
public class PlayerFootsteps : MonoBehaviour
{
    #region InspectorVariables
    // Variables in this region are set in the inspector

    [SerializeField] private float stepSpeedMultiplier = 0.5f;  // How regularly step sounds are played

    #endregion

    private PlayerMovement  playerMovement; // Reference to the player movement script for getting player velocity
    private float           stepTimer;      // Keeps track of the amount of time (seconds) since the last step

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(playerMovement.PlayerIsGrounded() && playerMovement.PlayerIsMoving())
        {
            // Player is on the ground and moving

            // Get the player's movement speed
            float playerSpeed = Vector3.Magnitude(playerMovement.GetVelocity());

            // Calculate the interval between step sounds based on player speed
            float stepInterval = 1.0f / (playerSpeed  * stepSpeedMultiplier);

            // Reset the step timer each time the interval is reached
            if(stepTimer >= stepInterval)
            {
                stepTimer = 0.0f;
            }

            // Play a sound whenever the timer is reset, the sound is played at 0.0 rather than stepInterval
            //   so a sound is instantly played as soon as they start moving, rather than after a small delay
            if (stepTimer == 0.0f)
            {
                AudioManager.Instance.PlaySoundEffect2D("footstep");
            }

            // Increment the step timer
            stepTimer += Time.deltaTime;
        }
        else
        {
            // Player is not moving/not on the ground, reset step timer
            stepTimer = 0.0f;
        }
    }
}
