using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    [Header("Puzzle Button")]
    [SerializeField] private bool               playerCanActivate     = true;
    [SerializeField] private bool               movableObjCanActivate = true;
    [SerializeField] private DoorPuzzleData[]   connectedDoors;

    [SerializeField] private Animator   animator;

    private bool playerIsColliding;
    private bool movableObjIsColliding;

    private bool lastFramePressed;
    private bool pressed;

    void Start()
    {
        ButtonReleasedEvents();
    }

    void Update()
    {
        pressed = (playerIsColliding || movableObjIsColliding);

        if(pressed && !lastFramePressed)
        {
            //Button was pressed this frame
            ButtonPressedEvents();
        }
        else if(!pressed && lastFramePressed)
        {
            //Button was released this frame
            ButtonReleasedEvents();
        }

        animator.SetBool("Pressed", pressed);

        lastFramePressed = pressed;
    }

    private void ButtonPressedEvents()
    {
        for (int i = 0; i < connectedDoors.Length; i++)
        {
            DoorPuzzleData doorData = connectedDoors[i];
            if (doorData.OpenByDefault)
            {
                doorData.Door.SetAsClosed();
            }
            else
            {
                doorData.Door.SetAsOpen(doorData.OpenInwards);
            }
        }
    }

    private void ButtonReleasedEvents()
    {
        for (int i = 0; i < connectedDoors.Length; i++)
        {
            DoorPuzzleData doorData = connectedDoors[i];
            if (doorData.OpenByDefault)
            {
                doorData.Door.SetAsOpen(doorData.OpenInwards);
            }
            else
            {
                doorData.Door.SetAsClosed();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerCanActivate && other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            playerIsColliding = true;
        }
        else if (movableObjCanActivate && other.CompareTag("MovableObj"))
        {
            movableObjIsColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerCanActivate && other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            playerIsColliding = false;
        }
        else if (movableObjCanActivate && other.CompareTag("MovableObj"))
        {
            movableObjIsColliding = false;
        }
    }
}

[System.Serializable]
public struct DoorPuzzleData
{
    public DoorMain Door;
    public bool     OpenInwards;
    public bool     OpenByDefault;
}