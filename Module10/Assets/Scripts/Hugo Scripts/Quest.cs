using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField]
    private string _questName;
    [SerializeField]
    private string _questDescription;

    public string questName
    {
        get { return _questName; }
    }
    public string questDescription
    {
        get { return _questDescription; }
    }

    [SerializeField]
    private List<Objective> objectives = new List<Objective>(); //change to objectives list

    [SerializeField]
    private GameObject rewardItem; //change to inventory item, maybe add gold / XP??

    public bool completed = false;
    private GameObject questGiver; //change to questGiver obj

    public bool checkObjectiveCompletion()
    {
        int complete = 0;

        if(objectives.Count > 0)
        {
            foreach(Objective objectiveNotYetComplete in objectives)
            {
                if(objectiveNotYetComplete.checkComplete())
                {
                    ++complete;
                    objectiveNotYetComplete.completed = true;
                }
            }

            return (complete == objectives.Count);
        }

        return false;
    }


}
