using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeldItemManager : MonoBehaviour
{
    private Item heldItem;
    private HotbarPanel hotbarPanel;
    private Transform playerTransform;

    private GameObject heldGameObject;
    private HeldItem heldItemScript;

    private float mouseHoldTimer;

    private const float mouseHoldThreshold = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        hotbarPanel = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<HotbarPanel>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.Find("Main Camera").transform;

        hotbarPanel.HeldItemChangedEvent += OnHeldItemSelectionChanged;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerInput();
    }

    private void CheckForPlayerInput()
    {
        if(heldItemScript != null)
        {
            if (heldItem != null && heldGameObject != null)
            {
                if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (mouseHoldTimer < mouseHoldThreshold)
                    {
                        heldItemScript.PerformMainAbility();
                    }

                    if (heldItemScript.PerformingPuzzleAbility)
                    {
                        heldItemScript.EndPuzzleAbility();
                    }

                    mouseHoldTimer = 0.0f;
                }
                else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    mouseHoldTimer += Time.deltaTime;

                    if (!heldItemScript.PerformingPuzzleAbility && mouseHoldTimer > mouseHoldThreshold)
                    {
                        heldItemScript.StartPuzzleAbility();
                    }
                }
            }
            else
            {
                mouseHoldTimer = 0.0f;
            }
        }
    }

    public void OnHeldItemSelectionChanged(Item item)
    {
        Item oldHeldItem = heldItem;
        heldItem = item;

        if (heldItem != null)
        {
            if (oldHeldItem == null || heldItem.Id != oldHeldItem.Id)
            {
                Debug.Log("Player is holding " + heldItem.UIName);

                DestroyHeldGameObject();

                if (heldItem.HeldItemGameObject != null)
                {
                    heldGameObject = Instantiate(heldItem.HeldItemGameObject, playerTransform);
                    heldItemScript = heldGameObject.GetComponent<HeldItem>();

                    if(heldItemScript != null)
                    {
                        heldItemScript.Setup(heldItem);
                    }
                }
            }
        }
        else
        {
            DestroyHeldGameObject();

            Debug.Log("Player is holding nothing");
        }
    }

    private void DestroyHeldGameObject()
    {
        if (heldGameObject != null)
        {
            heldItemScript = null;
            Destroy(heldGameObject);
        }
    }
}
