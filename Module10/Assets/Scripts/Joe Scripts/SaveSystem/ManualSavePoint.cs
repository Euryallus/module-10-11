using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSavePoint : InteractableWithOutline, ISavePoint
{
    [Header("Save Point Properties")]

    [SerializeField] [Tooltip("Unique id for this save point. Important: all save points should use a different id.")]
    private string id;

    public Transform SpawnPlatformTransform;

    public string Id { get { return id; } }

    public override void Interact()
    {
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