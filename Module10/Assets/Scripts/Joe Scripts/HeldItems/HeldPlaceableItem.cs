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

    [SerializeField]
    protected float maxPlaceDistance = 10.0f;

    [SerializeField]
    private string placementSound;

    protected bool colliding;
    protected bool inRange;
    protected bool snapping;

    protected float rotation;
    protected float visualRotation;
    protected Vector3 placePos;

    protected virtual void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(0.0f, visualRotation, 0.0f);

        LayerMask layerMask = ~(LayerMask.GetMask("BuildPreview") | LayerMask.GetMask("Ignore Raycast"));

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, maxPlaceDistance, layerMask))
        {
            UpdatePlacementState(colliding, true, snapping);

            placePos = hitInfo.point;

            gameObject.transform.position = placePos;
        }
        else
        {
            UpdatePlacementState(colliding, false, snapping);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            rotation -= 30.0f;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            rotation += 30.0f;
        }

        //roundedRotation = ((int)(rotation / 15.0f) * 15.0f);

        visualRotation = Mathf.Lerp(visualRotation, rotation, Time.deltaTime * 40.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player") && !other.CompareTag("BuildPoint"))
        {
            if(!(snapping && other.CompareTag("ModularPiece")))
            {
                UpdatePlacementState(true, inRange, snapping);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("BuildPoint"))
        {
            if (!(snapping && other.CompareTag("ModularPiece")))
            {
                UpdatePlacementState(true, inRange, snapping);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("BuildPoint"))
        {
            UpdatePlacementState(false, inRange, snapping);
        }
    }

    protected void UpdatePlacementState(bool colliding, bool inRange, bool snapping)
    {
        this.colliding  = colliding;
        this.inRange    = inRange;
        this.snapping   = snapping;

        if(!colliding && inRange)
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

        float randomOffset = Random.Range(0.0f, 0.001f);
        Vector3 offsetPlacePos = new Vector3(placePos.x + randomOffset, placePos.y + randomOffset, placePos.z + randomOffset);

        return Instantiate(itemPrefab, offsetPlacePos, Quaternion.Euler(0.0f, rotation, 0.0f));
    }
}
