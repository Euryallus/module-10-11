using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest data", menuName = "ScriptableObjects/Quest Shit/New quest", order = 2)]
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
                    case QuestObjective.type.GoTo:
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
