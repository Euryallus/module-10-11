using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ShopTalkPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI    shopNameText;
    [SerializeField] private GameObject         buyUIPrefab;

    private CanvasGroup canvasGroup;
    private ShopNPC     currentNPC;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        
    }

    public void Show(ShopNPC npc)
    {
        shopNameText.text = npc.ShopType.UIName;

        currentNPC = npc;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
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
