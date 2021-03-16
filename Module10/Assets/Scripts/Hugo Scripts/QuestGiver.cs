using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private List<Quest> questsToGive;
    [SerializeField]
    private bool issueInOrder = true;

    private QuestManager questManager;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    public void issueQuest()
    {
        if(issueInOrder && questsToGive.Count != 0)
        {
            questManager.AcceptQuest(questsToGive[questsToGive.Count - 1]);
            questsToGive.RemoveAt(questsToGive.Count - 1);
        }
    }

}
