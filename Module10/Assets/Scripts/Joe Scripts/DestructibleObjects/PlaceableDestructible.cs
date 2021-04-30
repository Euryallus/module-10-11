using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableDestructible : DestructableObject, IPersistentPlacedObject
{
    protected bool placedByPlayer;

    protected override void Start()
    {
        base.Start();
    }

    protected virtual void OnDestroy()
    {
        if (placedByPlayer)
        {
            //This object no longer exists in the world, remove it from the save list
            WorldSave.Instance.RemovePlacedObjectFromSaveList(this);
        }
    }

    public virtual void SetupAsPlacedObject()
    {
        placedByPlayer = true;

        //Tell the WorldSave that this is a player-placed object that should be saved with the world
        WorldSave.Instance.AddPlacedObjectToSave(this);
    }

    public override void Destroyed()
    {
        if(placedByPlayer)
        {
            DestroyedByPlayer();
        }
        else
        {
            NotificationManager.Instance.AddNotificationToQueue(NotificationMessageType.CantDestroyObject);
        }
    }

    protected virtual void DestroyedByPlayer()
    {
        base.Destroyed();
    }

    public virtual void AddDataToWorldSave(SaveData saveData)
    {
    }
}
