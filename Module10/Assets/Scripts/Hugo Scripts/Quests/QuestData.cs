using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Quest data/New single quest", order = 1)]
[System.Serializable]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea(5, 10)]
    public string questDescription;

    [TextArea(5, 10)]
    public string questCompleteDialogue;

    [SerializeField]
    public List<QuestData> nextQuests = new List<QuestData>();

    public string handInNPCName;
    public string handOutNPCName;


    public bool questCompleted = false;
    public bool questHandedIn = false;

    public bool handInToGiver = true;

    public string questLineName = "";

    public List<ItemGroup> rewards = new List<ItemGroup>();
    public List<QuestObjective> objectives = new List<QuestObjective>();

    public bool CheckCompleted()
    {
        int objectiveCount = 0;
        foreach(QuestObjective task in objectives)
        {
            if(task.taskComplete)
            {
                ++objectiveCount;
            }
            else
            {
                if (task.checkCcompleted())
                {
                    task.taskComplete = true;
                    ++objectiveCount;
                }
            }
        }

        if(objectiveCount == objectives.Count)
        {
            questCompleted = true;
            return true;
        }

        return false;
        
    }
}
