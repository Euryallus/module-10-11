using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective
{
    //https://forum.unity.com/threads/quest-mission-system.300168/ help from LoftyTheMetroid user

    [SerializeField]
    protected string _objectiveName;

    public string objectiveName
    {
        get { return _objectiveName; }
    }

    virtual public bool checkComplete()
    {
        return false;
    }

}
