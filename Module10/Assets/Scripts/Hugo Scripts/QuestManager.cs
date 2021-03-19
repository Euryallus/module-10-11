using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private PlayerQuestBacklog playerQuestData;

    private QuestUI UI;

    private PlayerMovement playerMove;

    private void Start()
    {
        UI = gameObject.GetComponent<QuestUI>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();


        foreach(QuestData quest in playerQuestData.questBacklog)
        {
            UI.AddHUDQuestName(quest.questName);

            if(quest.questCompleted)
            {
                UI.SetHUDQuestNameCompleted(quest.questName);
            }
        }
    }

    public bool TalkToQuestGiver(QuestGiver giver)
    {
        for(int i = 0; i < playerQuestData.questBacklog.Count; i++)
        {
            QuestData quest = playerQuestData.questBacklog[i];
            if (quest.questCompleted && !quest.questHandedIn && quest.handInToGiver)
            {
                if(giver.checkQuestToHandIn(quest.questName))
                {
                    CompleteQuest(quest);

                    Debug.Log(quest.questLineName);

                    if(quest.nextQuests.Count != 0)
                    {
                        foreach(QuestData nextQuest in quest.nextQuests)
                        {
                            giver.AddQuest(nextQuest);
                        }
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public void OfferQuest(QuestData questToOffer, QuestGiver offerer)
    {
        playerMove.StopMoving();
        playerQuestData.pendingQuest = questToOffer;
        UI.DisplayQuestAccept(playerQuestData.pendingQuest);
        playerQuestData.offer = offerer;
    }

    public void AcceptQuest()
    {
        playerMove.StartMoving();
        playerQuestData.questBacklog.Add(playerQuestData.pendingQuest);
        UI.HideQuestAccept();
        playerQuestData.offer.PlayerAccepts();

        UI.AddHUDQuestName(playerQuestData.pendingQuest.questName);

        playerQuestData.offer = null;
        playerQuestData.pendingQuest = null;

    }

    public void DeclineQuest()
    {
        playerMove.StartMoving();
        UI.HideQuestAccept();
        playerQuestData.pendingQuest = null;
        playerQuestData.offer = null;
    }

    private void Update()
    {
        if(playerQuestData.questBacklog.Count != 0)
        {
            for(int i = 0; i < playerQuestData.questBacklog.Count; i++)
            {
                QuestData quest = playerQuestData.questBacklog[i];

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
        playerQuestData.questBacklog.Remove(quest);
        playerQuestData.completedQuests.Add(quest);

        UI.DisplayQuestComplete(quest);
        playerMove.StopMoving();
    }

}
