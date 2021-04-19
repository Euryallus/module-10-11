using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldSave : MonoBehaviour, IPersistentObject
{
    public string UsedSavePointId { get { return usedSavePointId; } set { usedSavePointId = value; } }

    [SerializeField] private GameObject signpostPrefab;

    public static WorldSave Instance;

    private string usedSavePointId;

    private List<IPersistentPlacedObject> placedObjectsToSave = new List<IPersistentPlacedObject>();

    private void Awake()
    {
        //Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected void Start()
    {
       SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    public void OnSave(SaveData saveData)
    {
        Debug.Log("Saving world save");

        if (!string.IsNullOrEmpty(usedSavePointId))
        {
            saveData.AddData("usedSavePointId", usedSavePointId);
        }
        else
        {
            Debug.LogWarning("Trying to save without setting a UsedSavePointId!");
        }

        //int uniquePlacedObjectId = 0;

        for (int i = 0; i < placedObjectsToSave.Count; i++)
        {
            placedObjectsToSave[i].AddDataToWorldSave(saveData);//, ref uniquePlacedObjectId);
        }
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading world save");

        usedSavePointId = saveData.GetData<string>("usedSavePointId");
    }

    public void OnLoadConfigure(SaveData saveData)
    {
        var saveDataEntries = saveData.GetSaveDataEntries();

        for (int i = 0; i < saveDataEntries.Count; i++)
        {
            var currentElement = saveDataEntries.ElementAt(i);

            switch (currentElement.Key)
            {
                case "sign":
                {
                    SignpostSaveData data = (SignpostSaveData)currentElement.Value;

                    GameObject signGameObj = Instantiate(signpostPrefab, new Vector3(data.Position[0], data.Position[1], data.Position[2]),
                                                Quaternion.Euler(data.Rotation[0], data.Rotation[1], data.Rotation[2]));

                    signGameObj.GetComponent<Signpost>().SetRelatedItem(data.RelatedItemId);
                }
                break;
            }
        }

        if (!string.IsNullOrEmpty(usedSavePointId))
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");

            for (int i = 0; i < savePoints.Length; i++)
            {
                ISavePoint currentSavePoint = savePoints[i].GetComponent<ISavePoint>();

                string savePointId = currentSavePoint.GetSavePointId();

                if (savePointId == usedSavePointId)
                {
                    Debug.Log("Moving player to save point: " + savePointId);

                    //Move player to the position of the spawn transform at the point they last saved
                    GameObject.FindGameObjectWithTag("Player").transform.position = currentSavePoint.GetRespawnPosition() + new Vector3(0.0f, 3.0f, 0.0f);
                }
            }
        }
    }

    public void AddPlacedObjectToSave(IPersistentPlacedObject objToSave)
    {
        placedObjectsToSave.Add(objToSave);
    }

    public void RemovePlacedObjectFromSaveList(IPersistentPlacedObject objToRemove)
    {
        placedObjectsToSave.Remove(objToRemove);
    }
}