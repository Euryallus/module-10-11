using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableWithOutline, IPersistentObject
{
    [Header("Chest Properties")]
    [SerializeField] private bool           linkedChest;
    [SerializeField] private ChestPanel     chestPanel;
    [SerializeField] private ItemContainer  itemContainer;
    [SerializeField] private InventoryPanel playerInventoryPanel;

    public override void Interact()
    {
        //Setup the chest panel so its slot ui elements are linked to the itemcontainer slots for this speficic chest
        itemContainer.LinkSlotsToUI(chestPanel.slotsUI);

        for (int i = 0; i < chestPanel.slotsUI.Count; i++)
        {
            chestPanel.slotsUI[i].UpdateUI();
        }

        chestPanel.Show();

        playerInventoryPanel.Show(InventoryShowMode.InventoryOnly, 150.0f);
    }

    public void OnSave(SaveData saveData)
    {
        
    }

    public void OnLoadSetup(SaveData saveData)
    {

    }

    public void OnLoadConfigure(SaveData saveData)
    {

    }

}
