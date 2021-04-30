using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeldModularPiece : HeldPlaceableItem
{
    [Header("Modular Piece")]
    [SerializeField] private BuildPointType[] snapToPointTypes;

    private Coroutine setupBuildPointsCoroutine;

    private void OnDestroy()
    {
        if(setupBuildPointsCoroutine != null)
        {
            Debug.Log("Stopping build point coroutine");
            StopCoroutine(setupBuildPointsCoroutine);
        }
    }

    public override void Setup(Item item, ContainerSlotUI containerSlot)
    {
        base.Setup(item, containerSlot);

        setupBuildPointsCoroutine = StartCoroutine(SetupBuildPointsCoroutine());
    }

    private IEnumerator SetupBuildPointsCoroutine()
    {
        List<BuildPoint> buildPoints = WorldSave.Instance.PlacedBuildPoints;

        Debug.Log("Setting up " + buildPoints.Count + " build points");

        int count = 0;
        int numEnabled = 0;

        for (int i = 0; i < buildPoints.Count; i++)
        {
            if (snapToPointTypes.Contains(buildPoints[i].BuildPointType))
            {
                buildPoints[i].SetColliderEnabled(true);

                numEnabled++;
            }
            else
            {
                buildPoints[i].SetColliderEnabled(false);
            }

            count++;

            if(count % 500 == 0)
            {
                yield return null;
            }
        }

        setupBuildPointsCoroutine = null;

        Debug.Log("Build point setup done, " + numEnabled + " were enabled");
    }

    protected override void CameraRaycastHit(RaycastHit hitInfo)
    {
        if (hitInfo.collider.CompareTag("BuildPoint") &&
            snapToPointTypes.Contains(hitInfo.collider.gameObject.GetComponent<BuildPoint>().BuildPointType))
        {
            placePos = hitInfo.collider.transform.position;

            if(!snapping)
            {
                rotation = hitInfo.collider.transform.rotation.eulerAngles.y;
            }

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
}