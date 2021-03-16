using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    //Set in inspector
    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemCountPanel;
    [SerializeField] private TextMeshProUGUI    itemCountText;

    public InventoryItemStack ItemStack { get { return m_itemStack; } private set { m_itemStack = value; } }

    private InventoryItemStack m_itemStack = new InventoryItemStack();
    private void Start()
    {
        //Hide slot UI by default
        itemImage.gameObject    .SetActive(false);
        itemCountPanel          .SetActive(false);
        itemCountText.gameObject.SetActive(false);
    }


    public void UpdateUI(InventoryItem itemType)
    {
        int stackSize = m_itemStack.GetStackSize();

        if(stackSize > 0)
        {
            itemImage.sprite = itemType.GetSprite();
            itemImage.gameObject.SetActive(true);

            itemCountPanel.SetActive(true);

            itemCountText.text = stackSize.ToString();
            itemCountText.gameObject.SetActive(true);
        }
        else
        {
            itemImage.gameObject.SetActive(false);

            itemCountPanel.SetActive(false);

            itemCountText.gameObject.SetActive(false);
        }
    }
}
