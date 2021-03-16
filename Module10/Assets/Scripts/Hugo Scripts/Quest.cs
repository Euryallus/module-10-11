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
    private List<Objective> objectives; //change to objectives list

    [SerializeField]
    private GameObject rewardItem; //change to inventory item, maybe add gold / XP??

    private bool completed = false;
    private GameObject questGiver; //change to questGiver obj
    private GameObject completePoint; //change to questGiver obj or position?

    public bool checkObjectiveCompletion()
    {
        int complete = 0;
        foreach(Objective objectiveNotYetComplete in objectives)
        {
            if(objectiveNotYetComplete.checkComplete())
            {
                ++complete;
            }
        }

        return (complete == objectives.Count);
    }


}
