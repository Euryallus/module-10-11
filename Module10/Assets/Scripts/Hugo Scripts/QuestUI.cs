using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text questTitle;
    [SerializeField]
    private TMP_Text questDescription;
    [SerializeField]
    private TMP_Text questObjectives;
    [SerializeField]
    private TMP_Text questCompleteMessage;
    [SerializeField]
    private TMP_Text questReward;

    [SerializeField]
    private TMP_Text questTitleHUD;

    [SerializeField]
    private CanvasGroup questAcceptCanvasGroup;
    [SerializeField]
    private CanvasGroup questCompleteCanvasGroup;

    private void Start()
    {
         HideQuestAccept();
        HideQuestComplete();
    }

    public void DisplayQuestAccept(QuestData quest)
    {
        Cursor.lockState = CursorLockMode.None;
        questTitle.text = quest.questName;
        questDescription.text = quest.questDescription;

        string objectivesText = "";

        foreach(QuestObjective objective in quest.objectives)
        {
            objectivesText = objectivesText + "\n" + objective.taskName;
        }

        questObjectives.text = objectivesText;

        questAcceptCanvasGroup.alpha = 1;
        questAcceptCanvasGroup.blocksRaycasts = true;
    }

    public void HideQuestAccept()
    {
        Cursor.lockState = CursorLockMode.Locked;
        questAcceptCanvasGroup.alpha = 0;
        questAcceptCanvasGroup.blocksRaycasts = false;
    }

    public void DisplayQuestComplete(QuestData quest)
    {
        Cursor.lockState = CursorLockMode.None;

        questTitle.text = quest.questName;
        questCompleteMessage.text = quest.questCompleteDialogue;

        string rewardText = "ADD LATER";

        questReward.text = rewardText;

        questCompleteCanvasGroup.alpha = 1;
        questCompleteCanvasGroup.blocksRaycasts = true;
    }

    public void HideQuestComplete()
    {
        Cursor.lockState = CursorLockMode.Locked;
        questCompleteCanvasGroup.alpha = 0;
        questCompleteCanvasGroup.blocksRaycasts = false;
    }

    public void UpdateHUDQuestName(string name)
    {
        questTitleHUD.color = Color.white;
        questTitleHUD.text = name;
    }

    public void MarkHUDQuestComplete()
    {
        questTitleHUD.color = Color.green;
    }

}
