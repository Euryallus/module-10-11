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
    private TMP_Text questCompleteTitle;
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

    private List<TMP_Text> questNamesHUD = new List<TMP_Text>();

    [SerializeField]
    private GameObject questMarkerBackground;

    [SerializeField]
    private TMP_Text defaultName;

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

        questCompleteTitle.text = quest.questName;
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

    public void AddHUDQuestName(string name)
    {
        TMP_Text newName = Instantiate(defaultName);
        newName.transform.gameObject.SetActive(true);
        newName.transform.SetParent(questMarkerBackground.transform);

        if(questNamesHUD.Count != 0)
        {
            newName.rectTransform.anchoredPosition = new Vector2(defaultName.rectTransform.anchoredPosition.x, defaultName.rectTransform.anchoredPosition.y - (60 * questNamesHUD.Count));
        }
        else
        {
            newName.rectTransform.anchoredPosition = defaultName.rectTransform.anchoredPosition;
        }

        newName.text = name;

        questNamesHUD.Add(newName);
        
    }

    public void SetHUDQuestNameCompleted(string name)
    {
        foreach(TMP_Text questName in questNamesHUD)
        {
            if(questName.text == name)
            {
                questName.color = Color.green;
                return;
            }
        }
    }

    public void RemoveHUDQuestName(string name)
    {
        for(int i = 0; i < questNamesHUD.Count; i++)
        {
            if(questNamesHUD[i].text == name)
            {
                Destroy(questNamesHUD[i]);
                questNamesHUD.RemoveAt(i);

                if(questNamesHUD.Count != 0)
                {
                    for(int j = 0; j < questNamesHUD.Count; j ++)
                    {
                        questNamesHUD[j].rectTransform.anchoredPosition = new Vector2(defaultName.rectTransform.anchoredPosition.x, defaultName.rectTransform.anchoredPosition.y - (60 * j));
                    }
                }
                

                return;
            }

            
        }
    }

}
