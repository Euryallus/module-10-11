using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ItemInfoPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemCustomisedText;
    [SerializeField] private TextMeshProUGUI itemIdText;
    [SerializeField] private CanvasGroup     canvasGroup;

    private bool canShow;
    private bool showing;

    private void Start()
    {
        HidePopup();
    }

    public void SetCanShow(bool canShow)
    {
        if(!canShow && showing)
        {
            HidePopup();
        }

        this.canShow = canShow;
    }

    // Update is called once per frame
    void Update()
    {
        if (showing)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.0f, Time.unscaledDeltaTime * 25.0f);

            transform.position = Input.mousePosition;
        }
    }

    public void ShowPopup(string itemId)
    {
        Item item = ItemManager.Instance.GetItemWithID(itemId);

        if (canShow)
        {
            if (item != null)
            {
                UpdatePopupInfo(item.UIName, item.CustomItem, item.Id, item.BaseItemId);
            }
            else
            {
                UpdatePopupInfo("Error: Unknown Item", false, "unknown", "");
            }
        }

        showing = true;
    }

    public void HidePopup()
    {
        showing = false;

        canvasGroup.alpha = 0.0f;
    }

    private void UpdatePopupInfo(string itemName, bool customItem, string itemId, string baseItemId)
    {
        itemNameText.text = itemName;

        if (customItem)
        {
            itemCustomisedText.gameObject.SetActive(true);
            itemCustomisedText.text = ItemManager.Instance.GetItemWithID(baseItemId).UIName;
        }
        else
        {
            itemCustomisedText.gameObject.SetActive(false);
        }

        string baseIdText = customItem ? (": " + baseItemId) : "";

        itemIdText.text = "id: " + itemId + " " + baseIdText;
    }
}
