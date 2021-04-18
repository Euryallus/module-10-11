using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : InteractableWithOutline
{
    [Header("Shop")]

    [SerializeField] private string shopUIName;

    private NPCManager      npcManager;
    private PlayerMovement  playerMovement;
    private ShopTalkPanel   talkUI;

    private bool focusing;

    private void Awake()
    {
        playerMovement      = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        npcManager          = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<NPCManager>();
        talkUI              = GameObject.FindGameObjectWithTag("ShopTalkUI").GetComponent<ShopTalkPanel>();
    }

    public override void Interact()
    {
        if (!focusing)
        {
            base.Interact();

            playerMovement.StopMoving();

            npcManager.StartFocusCameraMove(transform.Find("FocusPoint"));

            Cursor.lockState = CursorLockMode.None;

            talkUI.Show(this, shopUIName);

            focusing = true;
        }
    }

    public void StopInteracting()
    {
        if (focusing)
        {
            playerMovement.StartMoving();

            npcManager.StopFocusCamera();

            Cursor.lockState = CursorLockMode.Locked;

            talkUI.Hide();

            focusing = false;
        }
    }
}
