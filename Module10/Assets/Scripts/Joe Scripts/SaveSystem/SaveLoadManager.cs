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

    [SerializeField] private GameObject loadingPanelPrefab;

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

    public void SubscribeSaveLoadEvents(Action<SaveData> onSave, Action<SaveData> onLoadSetup, Action<SaveData> onLoadConfig)
    {
        SaveObjectsEvent            += onSave;
        LoadObjectsSetupEvent       += onLoadSetup;
        LoadObjectsConfigureEvent   += onLoadConfig;
    }

    public void UnsubscribeSaveLoadEvents(Action<SaveData> onSave, Action<SaveData> onLoadSetup, Action<SaveData> onLoadConfig)
    {
        SaveObjectsEvent            -= onSave;
        LoadObjectsSetupEvent       -= onLoadSetup;
        LoadObjectsConfigureEvent   -= onLoadConfig;
    }

    public bool SaveGame()
    {
        SaveData dataToSave = new SaveData();

        //Call the OnSave function on all persistent objects - they will each add some data to the SavaData object
        SaveObjectsEvent?.Invoke(dataToSave);

        string sceneName = SceneManager.GetActiveScene().name;

        string saveDataPath = saveDirectory + "/" + sceneName + "_" + SaveDataFileName;

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
                return false;
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
            return false;
        }

        //Seralize the save data to the file
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, dataToSave);

        //Saving done, close the file
        file.Close();

        Debug.Log("Game saved!");

        return true;
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        Debug.Log("Attempting to load game");

        Transform canvasTransform = GameObject.FindGameObjectWithTag("JoeCanvas").transform;

        LoadingPanel        loadingPanel        = Instantiate(loadingPanelPrefab, canvasTransform).GetComponent<LoadingPanel>();
        CharacterController playerCharControl   = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        playerCharControl.enabled = false;

        string sceneName = SceneManager.GetActiveScene().name;

        string loadDataPath = saveDirectory + "/" + sceneName + "_" + SaveDataFileName;

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
                yield break;
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
                yield break;
            }

            yield return null;
            yield return null;

            //Setup then configure all persistent objects with the loaded data

            Debug.Log("Load Stage 1: Setup");
            LoadObjectsSetupEvent?.Invoke(loadedData);

            yield return null;

            Debug.Log("Load Stage 2: Configure");
            LoadObjectsConfigureEvent?.Invoke(loadedData);

            //Loading is done

            Debug.Log("Game loaded!");
        }
        else
        {
            Debug.LogWarning("No save file exists at: " + loadDataPath);
        }

        loadingPanel.LoadDone();

        playerCharControl.enabled = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        if(scene.name == "CombinedScene" || scene.name == "JoeTestScene" || scene.name == "Noah test scene" || scene.name == "DemoScene")
        {
            LoadGame();
        }
    }
}
