using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Info")]

    [SerializeField] [Tooltip("Unique identifier for this item")]
    private string id;

    [SerializeField] [Tooltip("Name to be displayed in the user interface")]
    private string uiName;

    [SerializeField] [Tooltip("Maximum number of this item that can be stored in a single stack")]
    private int stackSize = 1;

    [SerializeField] [Tooltip("Sprite to be displayed in the UI for this item")]
    private Sprite sprite;

    public string GetID()       { return id; }
    public int GetStackSize()   { return stackSize; }
    public Sprite GetSprite()   { return sprite; }
}
