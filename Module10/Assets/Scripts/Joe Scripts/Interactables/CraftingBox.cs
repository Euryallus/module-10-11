using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBox : InteractableWithOutline
{
    [Header("Crafting Box Properties")]
    public InventoryPanel inventoryPanel;

    public override void Interact()
    {
        base.Interact();

        if (!inventoryPanel.Showing)
        {
            inventoryPanel.Show(InventoryShowMode.Craft);
        }
    }
}