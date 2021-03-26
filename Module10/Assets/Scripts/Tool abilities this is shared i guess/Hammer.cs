using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : HeldItem
{
    [SerializeField] private GameObject prefabLaunchIndicator;

    private RaycastHit      raycastHit;
    private Transform       playerTransform;
    private Transform       playerCameraTransform;
    private PlayerMovement  playerMovementScript;
    private GameObject      launchIndicator;
    private float           launchTimer;
    private bool            launched;

    private const float launchDelay = 0.5f;
    private const float indicatorShrinkSpeed = 1.8f;

    private void Awake()
    {
        playerTransform         = GameObject.FindGameObjectWithTag("Player").transform;
        playerCameraTransform   = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerMovementScript    = playerTransform.GetComponent<PlayerMovement>();
    }

    public override void PerformMainAbility()
    {
        base.PerformMainAbility();

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out raycastHit, 4.0f))
        {
            DestructableObject destructable = raycastHit.transform.gameObject.GetComponent<DestructableObject>();

            if (destructable != null)
            {
                foreach (Item tool in destructable.toolToBreak)
                {
                    if (tool.Id == item.BaseItemId)
                    {
                        destructable.TakeHit();
                        break;
                    }
                }
            }
        }
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
