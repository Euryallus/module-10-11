using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [Header("Puzzle Button")]
    [SerializeField] private bool               playerCanActivate     = true;   //Whether the player can stand on the button to press it
    [SerializeField] private bool               movableObjCanActivate = true;   //Whether movable objects can be placed on the button to press it
    [SerializeField] private DoorPuzzleData[]   connectedDoors;                 //Doors that will be opened/closed by the button
    [SerializeField] private Animator           animator;                       //Controls button press/release animations

    #endregion

    private bool playerIsColliding;     //True if the player is colliding with the button
    private bool movableObjIsColliding; //True if any movable object is colliding with the button

    private bool lastFramePressed;      //Whether the button was pressed on the previous frame
    private bool pressed;               //Whether the button is currently pressed
    private PuzzleButtonSequence sequence;

    void Start()
    {
        //Ensure all connected doors are in the released button state by default
        ButtonReleasedEvents();
    }

    void Update()
    {
        //The button is pressed if the player or a movable object is colliding with it
        pressed = (playerIsColliding || movableObjIsColliding);

        if(pressed && !lastFramePressed)
        {
            //Button was pressed this frame - trigger events
            ButtonPressedEvents();
        }
        else if(!pressed && lastFramePressed)
        {
            //Button was released this frame - trigger events
            ButtonReleasedEvents();
        }

        //Update the animator Pressed property so the button's current state will be reflected visually
        animator.SetBool("Pressed", pressed);

        lastFramePressed = pressed;
    }

    public void RegisterWithSequence(PuzzleButtonSequence sequence)
    {
        this.sequence = sequence;
    }

    private void ButtonPressedEvents()
    {
        for (int i = 0; i < connectedDoors.Length; i++)
        {
            //Open/close all doors depending on their default states
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

        if(sequence != null)
        {
            sequence.ButtonInSequencePressed(this);
        }
    }

    private void ButtonReleasedEvents()
    {
        for (int i = 0; i < connectedDoors.Length; i++)
        {
            //Close/open all doors depending on their default states
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
            //The player entered the trigger and can press the button
            playerIsColliding = true;
        }
        else if (movableObjCanActivate && other.CompareTag("MovableObj"))
        {
            //A movable object entered the trigger and can press the button
            movableObjIsColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerCanActivate && other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            //The player exited the trigger and was pressing the button
            playerIsColliding = false;
        }
        else if (movableObjCanActivate && other.CompareTag("MovableObj"))
        {
            //A movable object exited the trigger and was pressing the button
            movableObjIsColliding = false;
        }
    }
}


//DoorPuzzleData defines how button presses will affect a certain door
[System.Serializable]
public struct DoorPuzzleData
{
    public DoorMain Door;           //The door to be affected by the puzzle element
    public bool     OpenInwards;    //Whether the door will open inwards or outwards
    public bool     OpenByDefault;  //Whether the door's default state will be open or closed
}