using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Quest data/New single quest", order = 1)]
[System.Serializable]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea(10, 10)]
    public string questDescription;

    [TextArea(10, 10)]
    public string questCompleteDialogue;


    public bool questCompleted = false;
    public bool questHandedIn = false;

    public bool handInToGiver = true;

    public string questLineName = "";

    public List<InventoryItem> rewards = new List<InventoryItem>();
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
                switch(task.objectiveType)
                {
                    case QuestObjective.Type.GoTo:
                        GoToQuestObjective o = (GoToQuestObjective)task;
                        
                        if(o.checkCcompleted())
                        {
                            task.taskComplete = true;
                            ++objectiveCount;
                        }

                        break;

                    default:
                        if (task.checkCcompleted())
                        {
                            task.taskComplete = true;
                            ++objectiveCount;
                        }
                        
                        break;
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
