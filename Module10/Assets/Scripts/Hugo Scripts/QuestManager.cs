using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    private Quest activeQuest;

    [SerializeField]
    private List<Quest> questBacklog;

    private List<Quest> completedQuests;

    [SerializeField]
    private Text questName;
    [SerializeField]
    private Text questDescription;

    public void AcceptQuest(Quest questAccepted)
    {
        questBacklog.Add(questAccepted);
        activeQuest = questBacklog[questBacklog.Count - 1];

        questName.text = activeQuest.questName;
        questDescription.text = activeQuest.questDescription;

    }

    private void Update()
    {
        foreach(Quest questNotCompleted in questBacklog)
        {
            if(questNotCompleted.checkObjectiveCompletion())
            {
                Debug.Log("COMPLETED " + questNotCompleted.questName);
            }
        }
    }
}
