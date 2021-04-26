using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoSaveArea : MonoBehaviour, ISavePoint, IPersistentObject
{
    #region InspectorVariables
    //Variables in this region are set in the inspector. See tooltips for more info.

    [SerializeField] [Header("Important: Set unique id")]
    [Tooltip("Unique id for this save point. Important: all save points should use a different id.")]
    private string id;

    [SerializeField]
    private bool disableWhenUsed = true;

    #endregion

    public string Id { get { return id; } }

    private bool        colliderDisabled;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        colliderDisabled = !boxCollider.enabled;
    }

    private void Start()
    {
        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("IMPORTANT: AutoSaveArea exists without id. All save points require a *unique* id for saving/loading data. Click this message to view the problematic GameObject.", gameObject);
        }
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    public void OnSave(SaveData saveData)
    {
        Debug.Log("Saving data for AutoSaveArea: " + id);

        saveData.AddData("saveColliderDisabled_" + id, colliderDisabled);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        Debug.Log("Loading data for AutoSaveArea: " + id);

        bool disableOnLoad = saveData.GetData<bool>("saveColliderDisabled_" + id);

        if (disableOnLoad)
        {
            DisableCollider();
        }
        else
        {
            EnableCollider();
        }
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (disableWhenUsed)
        {
            DisableCollider();
        }

        Debug.Log("Attempting to save game at save auto save point: " + id);

        WorldSave.Instance.UsedSavePointId = id;

        bool saveSuccess = SaveLoadManager.Instance.SaveGame();

        if (saveSuccess)
        {
            NotificationManager.Instance.ShowNotification(NotificationTextType.AutoSaveSuccess);
        }
        else
        {
            NotificationManager.Instance.ShowNotification(NotificationTextType.SaveError);
        }
    }

    public void DisableCollider()
    {
        colliderDisabled = true;
        boxCollider.enabled = false;
    }

    public void EnableCollider()
    {
        colliderDisabled = false;
        boxCollider.enabled = true;
    }

    public string GetSavePointId()
    {
        return id;
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }
}
