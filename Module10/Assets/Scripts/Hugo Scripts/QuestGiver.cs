using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : PersistentObject
{
    [SerializeField]
    private List<QuestData> questsToGive = new List<QuestData>();

    [SerializeField]
    private List<QuestLine> questLines = new List<QuestLine>();

    [SerializeField]
    public List<string> questsToAccept = new List<string>();

    [SerializeField]
    private bool issueInOrder = true;

    private QuestManager questManager;

    private QuestGiver self;

    public override void OnSave(SaveData saveData)
    {
        saveData.AddData("questsToGive", questsToGive);

        saveData.AddData("questlinesToGive", questLines);
        saveData.AddData("questsToAccept", questsToAccept);
        //throw new System.NotImplementedException();
    }

    public override void OnLoadConfigure(SaveData saveData)
    {
        questsToGive = saveData.GetData<List<QuestData>>("questsToGive");

        questLines = saveData.GetData<List<QuestLine>>("questlinesToGive");
        questsToAccept = saveData.GetData<List<string>>("questsToAccept");

    }

    public override void OnLoadSetup(SaveData saveData)
    {
        //throw new System.NotImplementedException();
    }

    protected override void Start()
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
            for(int nameID = 0; nameID < questsToAccept.Count; nameID ++)
            {
                string name = questsToAccept[nameID];
                
                if(questname == name)
                {
                    questsToAccept.RemoveAt(nameID);

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
