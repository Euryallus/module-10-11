using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Dictionary<string, object> saveDataEntries = new Dictionary<string, object>();

    public void AddData<T>(string id, T data)
    {
        if (!saveDataEntries.ContainsKey(id))
        {
            saveDataEntries.Add(id, data);
        }
        else
        {
            Debug.LogError("Trying to save multiple values with same id: " + id);
        }
    }

    public T GetData<T>(string id)
    {
        if (saveDataEntries.ContainsKey(id))
        {
            return (T)saveDataEntries[id];
        }
        else
        {
            Debug.LogError("Trying to load data with invalid id: " + id);
            return default;
        }
    }
}