using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public InventoryItemStack ItemStack { get { return itemStack; } private set { itemStack = value; } }

    //Set in inspector
    [SerializeField] private Image              itemImage;
    [SerializeField] private GameObject         itemCountPanel;
    [SerializeField] private TextMeshProUGUI    itemCountText;

    private InventoryItemStack itemStack = new InventoryItemStack();

    private void Start()
    {
        //Hide slot UI by default
        itemImage.gameObject    .SetActive(false);
        itemCountPanel          .SetActive(false);
        itemCountText.gameObject.SetActive(false);
    }


    public void UpdateUI(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
        {
            InventoryItem item = ItemManager.Instance.GetItemWithID(itemId);
            itemImage.sprite = item.GetSprite();
        }

        int stackSize = itemStack.StackSize;

        itemCountText.text = stackSize.ToString();

        itemImage.gameObject    .SetActive(stackSize > 0);
        itemCountPanel          .SetActive(stackSize > 1);
        itemCountText.gameObject.SetActive(stackSize > 1);
    }
}
