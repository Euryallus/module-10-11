using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Quest data/New questline", order = 2)]
public class QuestLine : ScriptableObject
{
    [SerializeField]
    private List<QuestData> questLineData = new List<QuestData>();

    private List<QuestData> completedQuests = new List<QuestData>();


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

            questLineData[0].questLineName = questLineName;

            completedQuests.Add(questLineData[0]);

            questLineData.RemoveAt(0);

            return completedQuests[completedQuests.Count - 1];
        }

        return null;
    }
}
