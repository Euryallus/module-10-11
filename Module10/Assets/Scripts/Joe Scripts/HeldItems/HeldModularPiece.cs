using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeldModularPiece : HeldPlaceableItem
{
    [Header("Modular Piece")]
    [SerializeField] private BuildPointType[] snapToPointTypes;

    protected override void Update()
    {
        //base.Update();

        gameObject.transform.rotation = Quaternion.Euler(0.0f, visualRotation, 0.0f);

        LayerMask layerMask = ~(LayerMask.GetMask("BuildPreview") | LayerMask.GetMask("Ignore Raycast"));

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, maxPlaceDistance, layerMask))
        {
            if (hitInfo.collider.CompareTag("BuildPoint") &&
                snapToPointTypes.Contains(hitInfo.collider.gameObject.GetComponent<BuildPoint>().BuildPointType))
            {
                placePos = hitInfo.collider.transform.position;
                rotation = hitInfo.collider.transform.rotation.eulerAngles.y;

                UpdatePlacementState(colliding, true, true);
            }
            else
            {
                placePos = hitInfo.point;

                UpdatePlacementState(colliding, true, false);
            }

            gameObject.transform.position = placePos;
        }
        else
        {
            UpdatePlacementState(colliding, false, false);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            rotation -= 30.0f;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            rotation += 30.0f;
        }

        visualRotation = Mathf.Lerp(visualRotation, rotation, Time.deltaTime * 40.0f);
    }
}
