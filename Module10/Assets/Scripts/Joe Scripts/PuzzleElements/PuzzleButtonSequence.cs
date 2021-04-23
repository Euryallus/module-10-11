using System.Collections.Generic;
using UnityEngine;

public class PuzzleButtonSequence : MonoBehaviour, IPersistentObject
{
    [Header("Puzzle Button Sequence")]

    [SerializeField] private string                 id;
    [SerializeField] private List<PuzzleButton>     buttonsInSequence;
    [SerializeField] private DoorPuzzleData[]       connectedDoors;         //Doors that will be opened/closed when the sequence is complete

    private int currentSequenceIndex = 0;

    private bool sequenceCompleted;

    private void Awake()
    {
        for (int i = 0; i < buttonsInSequence.Count; i++)
        {
            buttonsInSequence[i].RegisterWithSequence(this);
        }
    }

    void Start()
    {
        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("IMPORTANT: PuzzleButtonSequence exists without id. All sequences require a *unique* id for saving/loading data.");
        }

        SequenceFailedEvents();
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    public void OnSave(SaveData saveData)
    {
        saveData.AddData("buttonSequenceCompleted_" + id, sequenceCompleted);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        sequenceCompleted = saveData.GetData<bool>("buttonSequenceCompleted_" + id);

        if (sequenceCompleted)
        {
            SequenceCompleteEvents();
        }
        else
        {
            SequenceFailedEvents();
        }
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    public void ButtonInSequencePressed(PuzzleButton button)
    {
        if(buttonsInSequence.IndexOf(button) == currentSequenceIndex)
        {
            currentSequenceIndex++;

            if(currentSequenceIndex == buttonsInSequence.Count)
            {
                sequenceCompleted = true;

                AudioManager.Instance.PlaySoundEffect2D("notification1");

                SequenceCompleteEvents();
            }
            else
            {
                AudioManager.Instance.PlaySoundEffect2D("coins");
            }
        }
        else
        {
            sequenceCompleted = false;

            currentSequenceIndex = 0;

            AudioManager.Instance.PlaySoundEffect2D("sealExplosion");

            SequenceFailedEvents();
        }
    }

    private void SequenceCompleteEvents()
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
    }

    private void SequenceFailedEvents()
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
}
