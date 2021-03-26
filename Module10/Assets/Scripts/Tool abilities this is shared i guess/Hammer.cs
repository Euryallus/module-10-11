using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : HeldItem
{
    [SerializeField] private GameObject prefabLaunchIndicator;

    private PlayerMovement  playerMovementScript;
    private GameObject      launchIndicator;
    private float           launchTimer;
    private bool            launched;

    private const float launchDelay = 0.5f;
    private const float indicatorShrinkSpeed = 1.8f;

    protected override void Awake()
    {
        base.Awake();

        playerMovementScript = playerTransform.GetComponent<PlayerMovement>();
    }

    public override void StartPuzzleAbility()
    {
        if (playerMovementScript.PlayerIsGrounded())
        {
            base.StartPuzzleAbility();

            launchTimer = 0.0f;

            launchIndicator = Instantiate(prefabLaunchIndicator, playerTransform);
        }
    }

    public override void EndPuzzleAbility()
    {
        base.EndPuzzleAbility();

        if(launchIndicator != null)
        {
            Destroy(launchIndicator);
        }

        launched = false;
    }

    private void Update()
    {
        if (performingPuzzleAbility)
        {
            launchTimer += Time.deltaTime;

            if(launchTimer >= launchDelay && !launched)
            {
                LaunchPlayer();
            }
            else if(launchIndicator != null)
            {
                launchIndicator.transform.localScale -= new Vector3(Time.deltaTime * indicatorShrinkSpeed, 0.0f,
                                                                    Time.deltaTime * indicatorShrinkSpeed);
            }
        }
    }

    private void LaunchPlayer()
    {
        playerMovementScript.SetJumpVelocity(8.0f);   //Change to item.LaunchVelocity or similar

        if (launchIndicator != null)
        {
            Destroy(launchIndicator);
        }

        launched = true;
    }
}
