using UnityEngine;

// ||=======================================================================||
// || Chest: Stores items that the player puts into it and displays them in ||
// ||   a UI menu similar to the inventory.                                 ||
// ||=======================================================================||
// || Used on prefab: Joe/Environment/Crafting & Chests/Chest               ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

public class Chest : InteractableWithOutline
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [Header("Chest Properties")]
    [SerializeField] protected  ChestPanel     chestPanel;           // The UI panel that displays the contents of the chest
    [SerializeField] protected  ItemContainer  itemContainer;        // The ItemContainer that handles adding/removing/storing items
    [SerializeField] protected  InventoryPanel playerInventoryPanel; // 

    #endregion

    public override void Interact()
    {
        base.Interact();

        //Setup the chest panel so its slot ui elements are linked to the itemcontainer slots for this speficic chest
        itemContainer.LinkSlotsToUI(chestPanel.slotsUI);

        UpdateChestUI();

        chestPanel.Show(itemContainer.ContainerId == "linkedChest");

        playerInventoryPanel.Show(InventoryShowMode.InventoryOnly, 150.0f);
    }

    protected void UpdateChestUI()
    {
        for (int i = 0; i < chestPanel.slotsUI.Count; i++)
        {
            chestPanel.slotsUI[i].UpdateUI();
        }
    }
}