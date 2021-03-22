using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomiseBox : InteractableWithOutline
{
    [Header("Customise Box Properties")]
    public InventoryPanel inventoryPanel;

    public override void Interact()
    {
        if (!inventoryPanel.Showing)
        {
            inventoryPanel.Show(InventoryShowMode.Customise);
        }
    }
}
