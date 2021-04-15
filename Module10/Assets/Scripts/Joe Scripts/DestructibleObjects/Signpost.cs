using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Signpost : DestructableObject
{
    public string RelatedItemId { get { return relatedItemId; } set { relatedItemId = value; } }

    [SerializeField] private TextMeshPro text;

    private string relatedItemId = "signpost";

    public void SetSignText(string line1, string line2, string line3, string line4)
    {
        text.text = line1 + "\n" + line2 + "\n" + line3 + "\n" + line4;
    }

    public override void Destroyed()
    {
        base.Destroyed();

        Destroy(gameObject);

        InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();

        inventory.AddItemToInventory(relatedItemId);
    }
}