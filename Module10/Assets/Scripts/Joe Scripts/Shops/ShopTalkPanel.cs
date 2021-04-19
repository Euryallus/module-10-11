using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopTalkPanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI    shopNameText;
    [SerializeField] private GameObject         buyUIPrefab;

    private ShopNPC     currentNPC;

    protected override void Start()
    {
        base.Start();

        Hide();
    }

    void Update()
    {
        if (showing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ButtonLeave();
            }
        }
    }

    public void ShowAndSetup(ShopNPC npc)
    {
        Show();

        shopNameText.text = npc.ShopType.UIName;

        currentNPC = npc;
    }

    public void ButtonBuy()
    {
        Hide();

        GameObject buyPanel = Instantiate(buyUIPrefab, GameObject.FindGameObjectWithTag("JoeCanvas").transform);

        ShopBuyPanel panelScript = buyPanel.GetComponent<ShopBuyPanel>();

        panelScript.Setup(currentNPC);
    }

    public void ButtonSell()
    {
        
    }

    public void ButtonLeave()
    {
        currentNPC.StopInteracting();
    }
}
