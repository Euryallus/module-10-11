using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldPlaceableItem : HeldItem
{
    [Header("Placeable Item")]

    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private Color standardColour = new Color(0.0f, 0.6705883f, 0.6705883f);

    [SerializeField] [ColorUsage(false, true)]
    private Color standardEmissive = new Color(0.0f, 0.1254902f, 0.1254902f);

    [SerializeField]
    private Color warningColour = new Color(0.7075472f, 0.0f, 0.0f);

    [SerializeField] [ColorUsage(false, true)]
    private Color warningEmissive = new Color(0.1254902f, 0.0f, 0.0f);

    [SerializeField] protected float maxPlaceDistance = 10.0f;

    [SerializeField] private string placementSound;

    [SerializeField] private Collider mainCollider;
    [SerializeField] private Collider snapCollider;

    protected bool colliding;
    protected bool inRange;
    protected bool snapping;

    protected float rotation;
    protected float visualRotation;
    protected Vector3 placePos;
    private float angleInterval = DefaultAngleInterval;

    const float DefaultAngleInterval    = 30.0f;
    const float SnapAngleInterval       = 90.0f;

    protected CameraShake playerCameraShake;

    protected override void Awake()
    {
        base.Awake();

        playerCameraShake = playerTransform.GetComponent<CameraShake>();
    }

    private void Start()
    {
        mainCollider.enabled = true;
        snapCollider.enabled = false;
    }

    protected virtual void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(0.0f, visualRotation, 0.0f);

        LayerMask layerMask = ~(LayerMask.GetMask("BuildPreview") | LayerMask.GetMask("Ignore Raycast"));

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, maxPlaceDistance, layerMask))
        {
            CameraRaycastHit(hitInfo);
        }
        else
        {
            CameraRaycastNoHit();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            rotation -= angleInterval;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            rotation += angleInterval;
        }

        visualRotation = Mathf.Lerp(visualRotation, rotation, Time.deltaTime * 40.0f);
    }

    protected virtual void CameraRaycastHit(RaycastHit hitInfo)
    {
        SetInRange(true);

        placePos = hitInfo.point;

        gameObject.transform.position = placePos;
    }

    protected virtual void CameraRaycastNoHit()
    {
        SetInRange(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        SetColliding(true);
    }

    private void OnTriggerStay(Collider other)
    {
        SetColliding(true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetColliding(false);
    }

    protected void SetColliding(bool colliding)
    {
        this.colliding = colliding;

        UpdatePlacementState();
    }

    protected void SetInRange(bool inRange)
    {
        this.inRange = inRange;

        UpdatePlacementState();
    }

    protected void SetSnapping(bool snapping)
    {
        if((this.snapping && !snapping) || (!this.snapping && snapping))
        {
            angleInterval = snapping ? SnapAngleInterval : DefaultAngleInterval;

            this.snapping = snapping;

            mainCollider.enabled = !snapping;
            snapCollider.enabled = snapping;

            colliding = false;

            UpdatePlacementState();
        }
    }

    protected void UpdatePlacementState()
    {
        if (!colliding && inRange)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).CompareTag("BuildPreviewMaterial"))
                {
                    transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", standardColour);
                    transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", standardEmissive);
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("BuildPreviewMaterial"))
                {
                    transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", warningColour);
                    transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", warningEmissive);
                }
            }
        }
    }

    public override void PerformMainAbility()
    {
        base.PerformMainAbility();

        if(Cursor.lockState == CursorLockMode.Locked)
        {
            if (!colliding && inRange)
            {
                PlaceItem();
            }
            else
            {
                NotificationManager.Instance.ShowNotification(NotificationTextType.ItemCannotBePlaced);
            }
        }
    }

    protected virtual GameObject PlaceItem()
    {
        if(!string.IsNullOrEmpty(placementSound))
        {
            AudioManager.Instance.PlaySoundEffect3D(placementSound, transform.position);
        }

        //Tiny offset to help prevent z-fighting
        float randomOffset = Random.Range(0.0f, 0.001f);
        Vector3 offsetPlacePos = new Vector3(placePos.x + randomOffset, placePos.y + randomOffset, placePos.z + randomOffset);

        //Shake the player camera slightly
        playerCameraShake.ShakeCameraForTime(0.2f, CameraShakeType.ReduceOverTime, 0.02f);

        return Instantiate(itemPrefab, offsetPlacePos, Quaternion.Euler(0.0f, rotation, 0.0f));
    }
}
