using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObjective : ScriptableObject
{
    public string taskName;
    public bool taskComplete;

    public enum Type
    {
        GoTo
    }

    [HideInInspector]
    public Type objectiveType;

    public virtual bool checkCcompleted()
    {
        return false;
    }
}
