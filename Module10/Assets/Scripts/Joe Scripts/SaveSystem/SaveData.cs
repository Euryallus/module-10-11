using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    private Dictionary<string, object> saveDataEntries = new Dictionary<string, object>();

    public void AddData<T>(string id, T data)
    {
        //When multiple items of the same type (and hence same id, for example player-placed items) are saved, the * symbol can be used at the end of the id
        //  to show that duplication is intended and not an error (e.g. saving over the same inventory slot multiple times would be unintended behaviour)

        //Since a unique id is needed for the dictionary entry, an extra '*' is appended to the end of each id to differentiate between them

        if (id[id.Length - 1] == '*')
        {
            while (saveDataEntries.ContainsKey(id))
            {
                id += "*";
            }
        }

        if (!saveDataEntries.ContainsKey(id))
        {
            saveDataEntries.Add(id, data);
        }
        else
        {
            Debug.LogError("Trying to save multiple values with same id: " + id);
        }
    }

    public T GetData<T>(string id, out bool loadSuccess)
    {
        if (saveDataEntries.ContainsKey(id))
        {
            loadSuccess = true;
            return (T)saveDataEntries[id];
        }
        else
        {
            loadSuccess = false;
            Debug.LogError("Trying to load data with invalid id: " + id);
            return default;
        }
    }

    public T GetData<T>(string id)
    {
        return GetData<T>(id, out _);
    }

    public Dictionary<string, object> GetSaveDataEntries()
    {
        return saveDataEntries;
    }
}