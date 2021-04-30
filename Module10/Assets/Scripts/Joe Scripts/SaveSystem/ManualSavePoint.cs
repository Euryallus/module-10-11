using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSavePoint : InteractableWithOutline, ISavePoint
{
    [Header("Important: Set unique id")]
    [Header("Save Point Properties")]

    [SerializeField] [Tooltip("Unique id for this save point. Important: all save points should use a different id.")]
    private string id;

    public Transform SpawnPlatformTransform;

    public string Id { get { return id; } }

    protected override void Start()
    {
        base.Start();

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("IMPORTANT: ManualSavePoint exists without id. All save points require a *unique* id for saving/loading data. Click this message to view the problematic GameObject.", gameObject);
        }
    }

    public override void Interact()
    {
        base.Interact();

        Debug.Log("Attempting to save game at point: " + id);

        WorldSave.Instance.UsedSavePointId = id;

        bool saveSuccess = SaveLoadManager.Instance.SaveGame();

        if (saveSuccess)
        {
            NotificationManager.Instance.ShowNotification(NotificationTextType.SaveSuccess);
        }
        else
        {
            NotificationManager.Instance.ShowNotification(NotificationTextType.SaveError);
        }
    }

    public string GetSavePointId()
    {
        return id;
    }

    public Vector3 GetRespawnPosition()
    {
        return SpawnPlatformTransform.position;
    }
}