using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    public event Action<SaveData>   SaveObjectsEvent;
    public event Action<SaveData>   LoadObjectsSetupEvent;
    public event Action<SaveData>   LoadObjectsConfigureEvent;

    private string saveDirectory;

    private const string    SaveDataFileName    = "save.dat";

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

        saveDirectory = Application.persistentDataPath + "/Saves";

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SaveGame()
    {
        SaveData dataToSave = new SaveData();

        //Call the OnSave function on all persistent objects - they will each add some data to the SavaData object
        SaveObjectsEvent?.Invoke(dataToSave);

        string saveDataPath = saveDirectory + "/" + SaveDataFileName;

        //Try to create the save directory folder
        if (!Directory.Exists(saveDirectory))
        {
            try
            {
                Directory.CreateDirectory(saveDirectory);
            }
            catch
            {
                Debug.LogError("Could not create save directory: " + saveDirectory);
                return;
            }
        }

        FileStream file;

        //Try to open the save data file, or create one if it doesn't already exist
        try
        {
            file = File.Open(saveDataPath, FileMode.OpenOrCreate);
        }
        catch
        {
            Debug.LogError("Could not open/create file at " + saveDataPath);
            return;
        }

        //Seralize the save data to the file
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, dataToSave);

        //Saving done, close the file
        file.Close();

        Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        Debug.Log("Attempting to load game");

        string loadDataPath = saveDirectory + "/" + SaveDataFileName;

        if (File.Exists(loadDataPath))
        {
            FileStream file;

            try
            {
                file = File.OpenRead(loadDataPath);
            }
            catch
            {
                Debug.LogError("Could not open file when loading: " + loadDataPath);
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();

            SaveData loadedData;

            try
            {
                loadedData = (SaveData)bf.Deserialize(file);
                file.Close();
            }
            catch
            {
                file.Close();
                Debug.LogError("Could not deserialize save data from " + loadDataPath);
                return;
            }

            //Setup then configure all persistent objects with the loaded data

            Debug.Log("Load Stage 1: Setup");
            LoadObjectsSetupEvent?      .Invoke(loadedData);

            Debug.Log("Load Stage 2: Configure");
            LoadObjectsConfigureEvent?  .Invoke(loadedData);

            //Loading is done

            Debug.Log("Game loaded!");
        }
        else
        {
            Debug.LogError("File does not exist: " + saveDirectory);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        if(scene.name == "CombinedScene" || scene.name == "JoeTestScene")
        {
            StartCoroutine(LoadSceneCoroutine());
        }
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return null;
        yield return null;

        LoadGame();
    }
}
