using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : PersistentObject
{
    [SerializeField]
    private List<QuestData> questBacklog = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();

    private QuestData pendingQuest = null;
    private QuestGiver offer = null;

    private QuestUI UI;

    private PlayerMovement playerMove;

    public override void OnSave(SaveData saveData)
    {
        saveData.AddData("questsCompleted", completedQuests);
        saveData.AddData("questsInBacklog", questBacklog);
        //throw new System.NotImplementedException();
    }

    public override void OnLoadSetup(SaveData saveData)
    {
        completedQuests = saveData.GetData<List<QuestData>>("questsCompleted");
        questBacklog = saveData.GetData<List<QuestData>>("questsInBacklog");
        //throw new System.NotImplementedException();
    }

    public override void OnLoadConfigure(SaveData saveData)
    {
        //throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        UI = gameObject.GetComponent<QuestUI>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public bool TalkToQuestGiver(QuestGiver giver)
    {
        for(int i = 0; i < questBacklog.Count; i++)
        {
            QuestData quest = questBacklog[i];
            if (quest.questCompleted && !quest.questHandedIn && quest.handInToGiver)
            {
                if(giver.checkQuestToHandIn(quest.questName))
                {
                    CompleteQuest(quest);

                    Debug.Log(quest.questLineName);

                    if(quest.questLineName != "")
                    {
                        giver.ContinueQuestline(quest.questLineName);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public void offerQuest(QuestData questToOffer, QuestGiver offerer)
    {
        playerMove.StopMoving();
        pendingQuest = questToOffer;
        UI.DisplayQuestAccept(pendingQuest);
        offer = offerer;
    }

    public void AcceptQuest()
    {
        playerMove.StartMoving();
        questBacklog.Add(pendingQuest);
        UI.HideQuestAccept();
        offer.PlayerAccepts();

        UI.AddHUDQuestName(pendingQuest.questName);

        offer = null;
        pendingQuest = null;

    }

    public void DeclineQuest()
    {
        playerMove.StartMoving();
        UI.HideQuestAccept();
        pendingQuest = null;
        offer = null;
    }

    private void Update()
    {
        if(questBacklog.Count != 0)
        {
            for(int i = 0; i < questBacklog.Count; i++)
            {
                QuestData quest = questBacklog[i];

                if(!quest.questCompleted)
                {
                    if(quest.CheckCompleted())
                    {
                        Debug.Log("completed " + quest.questName);
                        
                        UI.SetHUDQuestNameCompleted(quest.questName);

                        if (!quest.handInToGiver)
                        {
                            CompleteQuest(quest);
                        }
                    }
                }
            }
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        UI.RemoveHUDQuestName(quest.questName);
        Debug.Log("REMOVED " + quest.questName);
        quest.questHandedIn = true;
        questBacklog.Remove(quest);
        completedQuests.Add(quest);

        UI.DisplayQuestComplete(quest);
        playerMove.StopMoving();
    }



}
