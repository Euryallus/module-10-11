using UnityEngine;

public class WorldSave : MonoBehaviour, IPersistentObject
{
    [SerializeField] private Transform spawnPlatformTransform;

    public string UsedSavePointId { get { return usedSavePointId; } set { usedSavePointId = value; } }

    public static WorldSave Instance;

    private string usedSavePointId;

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
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading world save");

        usedSavePointId = saveData.GetData<string>("usedSavePointId");
    }

    public void OnLoadConfigure(SaveData saveData)
    {
        if (!string.IsNullOrEmpty(usedSavePointId))
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");

            for (int i = 0; i < savePoints.Length; i++)
            {
                SavePoint currentSavePoint = savePoints[i].GetComponent<SavePoint>();

                if (currentSavePoint.Id == usedSavePointId)
                {
                    Debug.Log("Moving player to save point: " + currentSavePoint.Id);

                    //Move player to the position of the spawn transform at the point they last saved
                    GameObject.FindGameObjectWithTag("Player").transform.position = currentSavePoint.SpawnPlatformTransform.position + new Vector3(0.0f, 3.0f, 0.0f);
                }
            }
        }
    }
}