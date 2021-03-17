using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentObject : MonoBehaviour
{
    protected virtual void Start()
    {
        SaveLoadManager.Instance.SaveObjectsEvent += OnSave;
        SaveLoadManager.Instance.LoadObjectsEvent += OnLoad;
    }

    protected virtual void OnDestroy()
    {
        SaveLoadManager.Instance.SaveObjectsEvent -= OnSave;
        SaveLoadManager.Instance.LoadObjectsEvent -= OnLoad;
    }

    //OnLoad is called on this object when the game is loaded
    public abstract void OnLoad(SaveData saveData);

    //OnSave is called on this object when the game is saved
    public abstract void OnSave(SaveData saveData);

}