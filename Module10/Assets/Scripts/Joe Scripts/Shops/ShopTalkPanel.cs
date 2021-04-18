using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ShopTalkPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shopNameText;

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

    public void Show(ShopNPC npc, string shopName)
    {
        shopNameText.text = shopName;

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

    }

    public void ButtonSell()
    {
        
    }

    public void ButtonLeave()
    {
        currentNPC.StopInteracting();
    }
}
