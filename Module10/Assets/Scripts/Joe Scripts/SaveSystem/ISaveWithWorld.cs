using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistentPlacedObject
{
    public abstract void AddDataToWorldSave(SaveData saveData); //, ref int uniqueId);
}
