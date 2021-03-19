using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private List<QuestData> questsToGive = new List<QuestData>();

    [SerializeField]
    private List<QuestLine> questLines = new List<QuestLine>();

    [SerializeField]
    public List<string> questsToAccept;

    [SerializeField]
    private bool issueInOrder = true;

    private QuestManager questManager;

    private QuestGiver self;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        self = gameObject.GetComponent<QuestGiver>();

        for(int i = 0; i < questsToGive.Count; i++)
        {
            QuestData quest = questsToGive[i];
            if(quest.questHandedIn)
            {
                questManager.completedQuests.Add(quest);
                questsToGive.Remove(quest);
                --i;
            }
        }

        if(questLines.Count!= 0)
        {
            if(!questLines[0].completed)
            {
                questsToGive.Add(questLines[0].ReturnNextQuest());
            }

        }
    }

    public void Interact()
    {
        if(!questManager.TalkToQuestGiver(self))
        {
            if(questsToGive.Count > 0)
            {
                questManager.offerQuest(questsToGive[0], self);
            }
            else
            {
                Debug.Log("no quests left");
            }
        }
    }

    public void PlayerAccepts()
    {
        if(questsToGive[0].handInToGiver)
        {
            questsToAccept.Add(questsToGive[0].questName);
        }

        questsToGive.RemoveAt(0);
    }

    public bool checkQuestToHandIn(string questname)
    {
        if(questsToAccept.Count != 0)
        {
            foreach(string name in questsToAccept)
            {
                if(questname == name)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public void ContinueQuestline(string questLineName)
    {
        foreach(QuestLine questline in questLines)
        {
            if(questline.questLineName == questLineName)
            {
                if (!questline.completed)
                {
                    questsToGive.Add(questline.ReturnNextQuest());
                }

            }
        }
    }

    public void AddQuest(QuestData quest)
    {
        questsToGive.Add(quest);
    }
}
