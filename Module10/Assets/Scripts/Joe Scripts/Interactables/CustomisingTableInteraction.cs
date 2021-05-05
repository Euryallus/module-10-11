using UnityEngine;

public class CustomisingTableInteraction : InteractableWithOutline
{
    private InventoryPanel inventoryPanel;

    protected override void Start()
    {
        base.Start();

        inventoryPanel = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();
    }

    public override void Interact()
    {
        base.Interact();

        if (!inventoryPanel.Showing)
        {
            inventoryPanel.Show(InventoryShowMode.Customise);
        }
    }
}
