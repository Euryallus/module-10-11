using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    //[SerializeField]
    //private List<QuestData> questsToGive = new List<QuestData>();
    //
    //[SerializeField]
    //private List<QuestLine> questLines = new List<QuestLine>();
    //
    //[SerializeField]
    //public List<string> questsToAccept = new List<string>();

    [SerializeField]
    private QuestGiverData questGiverData;

    [SerializeField]
    private bool issueInOrder = true;

    private QuestManager questManager;

    private QuestGiver self;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        self = gameObject.GetComponent<QuestGiver>();


        if(questGiverData.questLines.Count!= 0)
        {
            if(!questGiverData.questLines[0].completed)
            {
                questGiverData.questsToGive.Add(questGiverData.questLines[0].ReturnNextQuest());
            }

        }
    }

    public void Interact()
    {
        if(!questManager.TalkToQuestGiver(self))
        {
            if(questGiverData.questsToGive.Count > 0)
            {
                questManager.offerQuest(questGiverData.questsToGive[0], self);
            }
            else
            {
                Debug.Log("no quests left");
            }
        }
    }

    public void PlayerAccepts()
    {
        if(questGiverData.questsToGive[0].handInToGiver)
        {
            questGiverData.questsToAccept.Add(questGiverData.questsToGive[0].questName);
        }

        questGiverData.questsToGive.RemoveAt(0);
    }

    public bool checkQuestToHandIn(string questname)
    {
        if(questGiverData.questsToAccept.Count != 0)
        {
            for(int nameID = 0; nameID < questGiverData.questsToAccept.Count; nameID ++)
            {
                string name = questGiverData.questsToAccept[nameID];
                
                if(questname == name)
                {
                    questGiverData.questsToAccept.RemoveAt(nameID);

                    return true;
                }
            }
        }
        
        return false;
    }

    public void ContinueQuestline(string questLineName)
    {
        foreach(QuestLine questline in questGiverData.questLines)
        {
            if(questline.questLineName == questLineName)
            {
                if (!questline.completed)
                {
                    questGiverData.questsToGive.Add(questline.ReturnNextQuest());
                }

            }
        }
    }

    public void AddQuest(QuestData quest)
    {
        questGiverData.questsToGive.Add(quest);
    }


}
