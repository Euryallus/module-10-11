using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldModularPiece : HeldPlaceableItem
{
    protected override void Update()
    {
        //base.Update();

        gameObject.transform.rotation = Quaternion.Euler(0.0f, visualRotation, 0.0f);

        LayerMask layerMask = ~(LayerMask.GetMask("BuildPreview") | LayerMask.GetMask("Ignore Raycast"));

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, maxPlaceDistance, layerMask))
        {
            if (hitInfo.collider.CompareTag("BuildPoint"))
            {
                placePos = hitInfo.collider.transform.position;
                rotation = hitInfo.collider.transform.rotation.eulerAngles.y;
            }
            else
            {
                placePos = hitInfo.point;
            }

            UpdatePlacementState(colliding, true);

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

        visualRotation = Mathf.Lerp(visualRotation, rotation, Time.deltaTime * 40.0f);
    }
}
