using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective
{
    //https://forum.unity.com/threads/quest-mission-system.300168/ help from LoftyTheMetroid user

    [SerializeField]
    protected string _objectiveName;

    [SerializeField]
    private Vector3 goToLocation;
    [SerializeField]
    private GameObject objectToCollect;
    [SerializeField]
    private int numberObjectToCollect;

    public bool completed = false;

    public enum objectiveType
    {
        Collect,
        GoTo
    }

    public objectiveType type;

    virtual public bool checkComplete()
    {
        switch (type)
        {
            case objectiveType.Collect:

                break;

            case objectiveType.GoTo:

                return (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, goToLocation) < 2);

            default:
                break;
        }


        return false;
    }

}
