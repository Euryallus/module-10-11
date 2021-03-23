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
    private NPCManager npcManager;

    private bool runupdate = true;

    private void Start()
    {
        UI = gameObject.GetComponent<QuestUI>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        npcManager = gameObject.GetComponent<NPCManager>();

        foreach (QuestData quest in playerQuestData.questBacklog)
        {
            UI.AddHUDQuestName(quest.questName);

            if(quest.questCompleted)
            {
                UI.SetHUDQuestNameCompleted(quest.questName);
            }
        }
    }

    public void OfferQuest(QuestData questToOffer, QuestGiverData offerer)
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
        playerQuestData.offer.questsToGive.RemoveAt(0);

        UI.AddHUDQuestName(playerQuestData.pendingQuest.questName);

        playerQuestData.offer = null;
        playerQuestData.pendingQuest = null;

        npcManager.StopFocusCamera();
    }

    public void DeclineQuest()
    {
        playerMove.StartMoving();
        UI.HideQuestAccept();
        playerQuestData.pendingQuest = null;
        playerQuestData.offer = null;

        npcManager.StopFocusCamera();
    }

    private void Update()
    {
        if(playerQuestData.questBacklog.Count > 0 && runupdate)
        {
            for(int i = 0; i < playerQuestData.questBacklog.Count; i++)
            {
                QuestData quest = playerQuestData.questBacklog[i];

                if(!(quest.questCompleted))
                {
                    if(quest.CheckCompleted())
                    {
                        Debug.Log("completed " + quest.questName);
                        
                        UI.SetHUDQuestNameCompleted(quest.questName);

                        //if (!quest.handInToGiver)
                        //{
                        //    CompleteQuest(quest);
                        //}
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

        playerMove.StopMoving();
        runupdate = false;

        UI.DisplayQuestComplete(quest);
    }

    public bool InteractWith(string questGiverName)
    {
        foreach(QuestGiverData giver in playerQuestData.questGivers)
        {
            if (giver.QuestGiverName == questGiverName)
            {
                for (int i = 0; i < playerQuestData.questBacklog.Count; i++)
                {
                    QuestData quest = playerQuestData.questBacklog[i];

                    if (quest.questCompleted && !quest.questHandedIn && quest.handInToGiver)
                    {
                        if (giver.QuestGiverName == quest.handInNPCName)
                        {
                            CompleteQuest(quest);

                            if (quest.nextQuests.Count != 0)
                            {
                                foreach (QuestData nextquest in quest.nextQuests)
                                {
                                    foreach (QuestGiverData q in playerQuestData.questGivers)
                                    {
                                        if (q.QuestGiverName == nextquest.handOutNPCName)
                                        {
                                            q.questsToGive.Add(nextquest);
                                        }
                                    }
                                }
                            }

                            return true;

                        }
                    }
                }

                if(giver.questsToGive.Count != 0)
                {
                    OfferQuest(giver.questsToGive[0], giver);
                    return true;
                }

            }
        }

        return false;
    }

    public void CloseQuestHandIn()
    {
        Debug.Log("Closed");
        UI.HideQuestComplete();

        runupdate = true;


        npcManager.StopFocusCamera();

        playerMove.StartMoving();
    }

}
