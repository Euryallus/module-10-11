using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldSave : MonoBehaviour, IPersistentObject
{
    public static WorldSave Instance;

    [SerializeField] private GameObject signpostPrefab;
    [SerializeField] private GameObject modularWoodFloorPrefab;
    [SerializeField] private GameObject modularWoodWallPrefab;
    [SerializeField] private GameObject modularWoodHalfWallPrefab;
    [SerializeField] private GameObject modularWoodRoofPrefab;
    [SerializeField] private GameObject modularWoodStairsPrefab;
    [SerializeField] private GameObject craftingTablePrefab;
    [SerializeField] private GameObject customisingTablePrefab;

    #region Properties

    public string           UsedSavePointId     { get { return usedSavePointId; } set { usedSavePointId = value; } }
    public List<BuildPoint> PlacedBuildPoints   { get { return placedBuildPoints; } }

    #endregion

    private string usedSavePointId;

    private List<IPersistentPlacedObject> placedObjectsToSave = new List<IPersistentPlacedObject>();

    private List<BuildPoint> placedBuildPoints = new List<BuildPoint>();

    private void Awake()
    {
        //Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            Instance = this;
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

        for (int i = 0; i < placedObjectsToSave.Count; i++)
        {
            placedObjectsToSave[i].AddDataToWorldSave(saveData);
        }
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading world save");

        usedSavePointId = saveData.GetData<string>("usedSavePointId");
    }

    public void OnLoadConfigure(SaveData saveData)
    {
        LoadPlayerPlacedObjects(saveData.GetSaveDataEntries());

        MovePlayerToSpawnPoint();
    }

    private void LoadPlayerPlacedObjects(Dictionary<string, object> saveDataEntries)
    {
        bool        loadingObjects  = true;
        string[]    idsToLoad       = new string[] { "sign", "modularPiece", "craftingTable" };
        int         idToLoadIndex   = 0;
        string      currentIdToLoad = idsToLoad[0];

        while (loadingObjects)
        {
            currentIdToLoad += "*";

            if (saveDataEntries.ContainsKey(currentIdToLoad))
            {
                var currentElement = saveDataEntries[currentIdToLoad];

                switch (idsToLoad[idToLoadIndex])
                {
                    case "sign":
                        LoadPlacedSignpost(currentElement as SignpostSaveData);
                        break;

                    case "modularPiece":
                        LoadPlacedModularPiece(currentElement as ModularPieceSaveData);
                        break;

                    case "craftingTable":
                        LoadPlacedCraftingTable(currentElement as TransformSaveData);
                        break;

                    case "customisingTable":
                        LoadPlacedCustomisingTable(currentElement as TransformSaveData);
                        break;
                }
            }
            else
            {
                if (idToLoadIndex < (idsToLoad.Length - 1))
                {
                    idToLoadIndex++;
                    currentIdToLoad = idsToLoad[idToLoadIndex];
                }
                else
                {
                    loadingObjects = false;
                }
            }
        }
    }

    private void LoadPlacedSignpost(SignpostSaveData data)
    {
        GameObject signGameObj = Instantiate(signpostPrefab, new Vector3(data.Position[0], data.Position[1], data.Position[2]),
                            Quaternion.Euler(data.Rotation[0], data.Rotation[1], data.Rotation[2]));

        Signpost signpostScript = signGameObj.GetComponent<Signpost>();

        signpostScript.SetupAsPlacedObject();
        signpostScript.SetRelatedItem(data.RelatedItemId);
    }

    private void LoadPlacedModularPiece(ModularPieceSaveData data)
    {
        GameObject prefabToSpawn = null;

        switch (data.PieceType)
        {
            case ModularPieceType.WoodFloor:
                prefabToSpawn = modularWoodFloorPrefab; break;

            case ModularPieceType.WoodWall:
                prefabToSpawn = modularWoodWallPrefab; break;

            case ModularPieceType.WoodHalfWall:
                prefabToSpawn = modularWoodHalfWallPrefab; break;

            case ModularPieceType.WoodRoofSide:
                prefabToSpawn = modularWoodRoofPrefab; break;

            case ModularPieceType.WoodStairs:
                prefabToSpawn = modularWoodStairsPrefab; break;
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, new Vector3(data.Position[0], data.Position[1], data.Position[2]),
                            Quaternion.Euler(data.Rotation[0], data.Rotation[1], data.Rotation[2]));
        }
        else
        {
            Debug.LogWarning("Trying to load modular piece with unknown prefab: " + data.PieceType);
        }
    }

    private void LoadPlacedCraftingTable(TransformSaveData data)
    {
        CraftingTable craftingTable = Instantiate(craftingTablePrefab, new Vector3(data.Position[0], data.Position[1], data.Position[2]),
                                            Quaternion.Euler(data.Rotation[0], data.Rotation[1], data.Rotation[2])).GetComponent<CraftingTable>();

        craftingTable.SetupAsPlacedObject();
    }
    
    private void LoadPlacedCustomisingTable(TransformSaveData data)
    {
        CustomisingTable customisingTable = Instantiate(customisingTablePrefab, new Vector3(data.Position[0], data.Position[1], data.Position[2]),
                                                Quaternion.Euler(data.Rotation[0], data.Rotation[1], data.Rotation[2])).GetComponent<CustomisingTable>();

        customisingTable.SetupAsPlacedObject();
    }

    private void MovePlayerToSpawnPoint()
    {
        if (!string.IsNullOrEmpty(usedSavePointId))
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");

            for (int i = 0; i < savePoints.Length; i++)
            {
                ISavePoint currentSavePoint = savePoints[i].GetComponent<ISavePoint>();

                string savePointId = currentSavePoint.GetSavePointId();

                if (savePointId == usedSavePointId)
                {
                    Debug.Log("Moving player to save point: " + savePointId + ", position: " + currentSavePoint.GetRespawnPosition());

                    //Move player to the position of the spawn transform at the point they last saved
                    GameObject.FindGameObjectWithTag("Player").transform.position = currentSavePoint.GetRespawnPosition() + new Vector3(0.0f, 3.0f, 0.0f);

                    break;
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

    public void AddPlacedBuildPoint(BuildPoint point)
    {
        placedBuildPoints.Add(point);
    }

    public void RemovePlacedBuildPoint(BuildPoint point)
    {
        placedBuildPoints.Remove(point);
    }
}

[Serializable]
public class TransformSaveData
{
    public float[] Position;
    public float[] Rotation;
}