using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ItemInfoPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemCustomisedText;
    [SerializeField] private TextMeshProUGUI customPropertiesText;
    //[SerializeField] private TextMeshProUGUI itemIdText;
    [SerializeField] private CanvasGroup     canvasGroup;

    private bool canShow;
    private bool showing;

    private Canvas canvas;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("JoeCanvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

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
            //Fade the popup in slightly each frame that it's showing
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.0f, Time.unscaledDeltaTime * 25.0f);
            
            //Get the current width/height of the popup
            float width =   (rectTransform.rect.width / 2)  * canvas.scaleFactor;
            float height =  (rectTransform.rect.height / 2) * canvas.scaleFactor;

            //Move the popup to the mouse pointer position, and clamp the position to be within screen bounds
            transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, width, Screen.width - width), Mathf.Clamp(Input.mousePosition.y, height, Screen.height - height));
        }
    }

    public void ShowPopup(string itemId)
    {
        Item item = ItemManager.Instance.GetItemWithID(itemId);

        if (canShow)
        {
            if (item != null)
            {
                UpdatePopupInfo(item.UIName, item.UIDescription, item.CustomItem, item.BaseItemId, item.CustomFloatProperties, item.CustomStringProperties);
            }
            else
            {
                UpdatePopupInfo("Error: Unknown Item", "", false, "", new CustomFloatProperty[] { }, new CustomStringProperty[] { });
            }
        }

        showing = true;
    }

    public void HidePopup()
    {
        showing = false;

        canvasGroup.alpha = 0.0f;
    }

    private void UpdatePopupInfo(string itemName, string itemDescription, bool customItem, string baseItemId,
                                    CustomFloatProperty[] customFloatProperties, CustomStringProperty[] customStringProperties)
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

        if (!string.IsNullOrWhiteSpace(itemDescription))
        {
            itemDescriptionText.gameObject.SetActive(true);
            itemDescriptionText.text = itemDescription;
        }
        else
        {
            itemDescriptionText.gameObject.SetActive(false);
        }

        if (customFloatProperties.Length > 0 || customStringProperties.Length > 0)
        {
            customPropertiesText.text = "";

            int shownPropertyCount = customFloatProperties.Length;

            for (int i = 0; i < customFloatProperties.Length; i++)
            {
                customPropertiesText.text += (customFloatProperties[i].UIName + ": " + customFloatProperties[i].Value);
                customPropertiesText.text += "\n";
            }

            for (int i = 0; i < customStringProperties.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(customStringProperties[i].Value))
                {
                    shownPropertyCount++;

                    string value = customStringProperties[i].Value;
                    if (value.Length > 10)
                    {
                        value = value.Remove(10);
                        value += "...";
                    }

                    customPropertiesText.text += (customStringProperties[i].UIName + ": " + value);
                    customPropertiesText.text += "\n";
                }
            }

            customPropertiesText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(150.0f, 16.0f * shownPropertyCount);

            customPropertiesText.gameObject.SetActive(true);
        }
        else
        {
            customPropertiesText.gameObject.SetActive(false);
        }
    }
}
