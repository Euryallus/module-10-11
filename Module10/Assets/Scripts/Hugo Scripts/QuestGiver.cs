using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private List<QuestData> questsToGive;

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

        foreach(QuestData quest in questsToGive) 
        {
            if(quest.questHandedIn)
            {
                questManager.completedQuests.Add(quest);
                questsToGive.Remove(quest);
            }
        }
    }

    public void interact()
    {
        if(!questManager.TalkToQuestGiver(self))
        {
            if(questsToGive.Count > 0)
            {
                questManager.offerQuest(questsToGive[0], self);
            }
        }
    }

    public void playerAccepts()
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
}
