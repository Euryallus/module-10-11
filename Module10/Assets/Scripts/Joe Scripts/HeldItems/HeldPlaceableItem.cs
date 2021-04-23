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
    private float maxPlaceDistance = 8.0f;

    private bool colliding = false;
    private bool inRange = false;

    private float rotation;
    private float visualRotation;
    private Vector3 placePos;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(0.0f, visualRotation, 0.0f);

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, maxPlaceDistance))
        {
            UpdatePlacementState(colliding, true);

            placePos = hitInfo.point;

            gameObject.transform.position = placePos;
        }
        else
        {
            UpdatePlacementState(colliding, false);
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
        UpdatePlacementState(true, inRange);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);
        UpdatePlacementState(true, inRange);
    }

    private void OnTriggerExit(Collider other)
    {
        UpdatePlacementState(false, inRange);
    }

    private void UpdatePlacementState(bool colliding, bool inRange)
    {
        this.colliding  = colliding;
        this.inRange    = inRange;

        if(!colliding && inRange)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", standardColour);
                transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", standardEmissive);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", warningColour);
                transform.GetChild(i).GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", warningEmissive);
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
        return Instantiate(itemPrefab, placePos, Quaternion.Euler(0.0f, visualRotation, 0.0f));
    }
}
