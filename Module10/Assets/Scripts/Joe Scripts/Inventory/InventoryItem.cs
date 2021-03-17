using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Info")]
    [Space]
    [Header("Hover over variable names for tooltips with more info.")]

    [SerializeField] [Tooltip("Unique identifier for this item")]
    private string id;

    [SerializeField] [Tooltip("Name to be displayed in the user interface")]
    private string uiName;

    [SerializeField] [Tooltip("Maximum number of this item that can be stored in a single stack")]
    private int stackSize = 1;

    [SerializeField] [Tooltip("The weight of this item - more desirable items should have a greater weight")]
    private float weight;

    [SerializeField] [Tooltip("Sprite to be displayed in the UI for this item")]
    private Sprite sprite;

    [Header("Player Customisation")]

    [SerializeField] [Tooltip("Whether or not the player can rename this item type")]
    private bool customisable;

    [SerializeField] [Tooltip("The id of the item needed to rename this item type. Leave blank if this item is not renamable, or does not require another item to be renamed.")]
    private string customiseItemId;

    [SerializeField] [Tooltip("The number of the above items required to rename this item type. Leave at 0 if this item is not renamable, or does not require another item to be renamed.")]
    private int customiseItemQuantity;

    public string   GetID()         { return id; }
    public int      GetStackSize()  { return stackSize; }
    public float    GetWeight()     { return weight; }
    public Sprite   GetSprite()     { return sprite; }

    public void SetUIName(string uiName)
    {
        this.uiName = uiName;
    }
}