using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeldModularPiece : HeldPlaceableItem
{
    [Header("Modular Piece")]
    [SerializeField] private BuildPointType[] snapToPointTypes;

    public override void Setup(Item item, ContainerSlotUI containerSlot)
    {
        base.Setup(item, containerSlot);

        GameObject[] buildPoints = GameObject.FindGameObjectsWithTag("BuildPoint");

        for (int i = 0; i < buildPoints.Length; i++)
        {
            if(snapToPointTypes.Contains(buildPoints[i].GetComponent<BuildPoint>().BuildPointType))
            {
                buildPoints[i].GetComponent<SphereCollider>().enabled = true;
            }
            else
            {
                buildPoints[i].GetComponent<SphereCollider>().enabled = false;
            }
        }
    }

    protected override void CameraRaycastHit(RaycastHit hitInfo)
    {
        if (hitInfo.collider.CompareTag("BuildPoint") &&
            snapToPointTypes.Contains(hitInfo.collider.gameObject.GetComponent<BuildPoint>().BuildPointType))
        {
            placePos = hitInfo.collider.transform.position;
            rotation = hitInfo.collider.transform.rotation.eulerAngles.y;

            SetInRange(true);
            SetSnapping(true);
        }
        else
        {
            placePos = hitInfo.point;

            SetInRange(true);
            SetSnapping(false);
        }

        gameObject.transform.position = placePos;
    }

    protected override void CameraRaycastNoHit()
    {
        base.CameraRaycastNoHit();

        SetSnapping(false);
    }

    protected override GameObject PlaceItem()
    {
        HotbarPanel hotbar = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<HotbarPanel>();

        hotbar.RemoveItemFromHotbar(item);

        return base.PlaceItem();
    }
}