using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup cg;

    [SerializeField]
    private TMP_Text dialogueText;

    //sets canvas group alpha to 1 and changes dialogue text component to display string passed
    public void ShowDialogue(string dialogue)
    {
        Cursor.lockState = CursorLockMode.None;
        cg.alpha = 1;
        cg.blocksRaycasts = true;

        dialogueText.text = dialogue;
    }

    //hides all dialogue
    public void HideDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cg.alpha = 0;
        cg.blocksRaycasts = false;

        dialogueText.text = "";
    }
}
