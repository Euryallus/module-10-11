using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private Quest activeQuest = null;

    [SerializeField]
    private List<Quest> questBacklog = new List<Quest>();

    private List<Quest> completedQuests = new List<Quest>();

    [SerializeField]
    private Text questName;
    [SerializeField]
    private Text questDescription;

    private void Start()
    {
        activeQuest = null;
    }

    public void AcceptQuest(Quest questAccepted)
    {
        questBacklog.Add(questAccepted);
        activeQuest = questBacklog[0];

        questName.text = activeQuest.questName;
        questDescription.text = activeQuest.questDescription;
    }

    private void Update()
    {
        if (activeQuest != null)
        { 
            if(!activeQuest.completed && activeQuest.checkObjectiveCompletion())
            {
                completedQuests.Add(questBacklog[0]);
                questBacklog[0].completed = true;
                questBacklog.RemoveAt(0);

                if (questBacklog.Count > 0)
                {
                    activeQuest = questBacklog[0];
                }
                else
                {
                    activeQuest = null;
                    questName.text = "no active quests";
                    questDescription.text = "";
                }
            }
        }
    }
}
