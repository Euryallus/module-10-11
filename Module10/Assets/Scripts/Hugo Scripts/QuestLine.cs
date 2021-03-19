using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Quest data/New questline", order = 2)]
public class QuestLine : ScriptableObject
{
    [SerializeField]
    private List<QuestData> questLineData = new List<QuestData>();


    public string questLineName;

    public bool completed = false;


    public QuestData ReturnNextQuest()
    {
        if(questLineData.Count != 0)
        {
            if(questLineData.Count == 1)
            {
                completed = true;
            }

            QuestData returnQuest = questLineData[0];
            returnQuest.questLineName = questLineName;

            questLineData.RemoveAt(0);

            return returnQuest;
        }

        return null;
    }
}
