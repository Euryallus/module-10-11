using UnityEngine;

public class HeldTool : HeldItem
{
    [Header("Food level reduction when item is used (for tools):")]
    [Header("Held Tool")]

    [SerializeField]
    [Tooltip("How much the player's food level decreases when the item's main ability is used")]
    protected float mainAbilityHunger;

    [SerializeField]
    [Tooltip("How much the player's food level decreases when the item's secondary ability is used")]
    protected float secondaryAbilityHunger;

    [SerializeField] protected SoundClass useToolSound;

    protected PlayerStats   playerStatsScript;
    protected RaycastHit    toolRaycastHit;
    protected CameraShake   playerCameraShake;

    public MeshRenderer toolRenderer;

    protected override void Awake()
    {
        base.Awake();

        playerStatsScript = playerTransform.GetComponent<PlayerStats>();
        playerCameraShake = playerTransform.GetComponent<CameraShake>();
    }

    public override void PerformMainAbility()
    {
        base.PerformMainAbility();

        //Check that the player cam isn't null, this can occur in certain cases when an alternate camera is being used (e.g. talking to an NPC)
        if (playerCameraTransform != null)
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out toolRaycastHit, 4.0f))
            {
                DestructableObject destructable = toolRaycastHit.transform.gameObject.GetComponent<DestructableObject>();

                if (destructable != null)
                {
                    foreach (Item tool in destructable.toolToBreak)
                    {
                        string compareId = item.CustomItem ? item.BaseItemId : item.Id;

                        if (tool.Id == compareId)
                        {
                            HitDestructibleObject(destructable, toolRaycastHit);

                            break;
                        }
                    }
                }
            }
        }
    }

    protected virtual void HitDestructibleObject(DestructableObject destructible, RaycastHit raycastHit)
    {
        playerStatsScript.DecreaseFoodLevel(mainAbilityHunger);

        destructible.TakeHit();
    }
}
