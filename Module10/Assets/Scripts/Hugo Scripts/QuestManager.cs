using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private List<QuestData> questBacklog = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();

    private QuestData pendingQuest = null;
    private QuestGiver offer = null;

    private QuestUI UI;

    private PlayerMovement playerMove;

    private void Start()
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
        offer.playerAccepts();
        UI.UpdateHUDQuestName(questBacklog[0].questName);

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

                        UI.MarkHUDQuestComplete();

                        if(!quest.handInToGiver)
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
        quest.questHandedIn = true;
        questBacklog.Remove(quest);
        completedQuests.Add(quest);

        if(questBacklog.Count != 0)
        {
            UI.UpdateHUDQuestName(questBacklog[0].questName);
        }
        else
        {
            UI.UpdateHUDQuestName("No active quests");
        }

        UI.DisplayQuestComplete(quest);
        playerMove.StopMoving();
    }

}
