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

    public override void StartSecondardAbility()
    {
        if (playerMovementScript.PlayerIsGrounded())
        {
            base.StartSecondardAbility();

            launchTimer = 0.0f;

            launchIndicator = Instantiate(prefabLaunchIndicator, playerTransform);
        }
    }

    public override void EndSecondaryAbility()
    {
        base.EndSecondaryAbility();

        if(launchIndicator != null)
        {
            Destroy(launchIndicator);
        }

        launched = false;
    }

    private void Update()
    {
        if (performingSecondaryAbility)
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
        playerStatsScript.DecreaseFoodLevel(secondaryAbilityHunger);

        float launchForce = item.GetCustomFloatPropertyWithName("LaunchForce").Value;

        playerMovementScript.SetJumpVelocity(launchForce);

        if (launchIndicator != null)
        {
            Destroy(launchIndicator);
        }

        launched = true;

        AudioManager.Instance.PlaySoundEffect2D(secondaryAbilitySound);
    }
}
