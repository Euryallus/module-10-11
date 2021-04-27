using UnityEngine;
using TMPro;

public class Signpost : DestructableObject, IPersistentPlacedObject
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private TextMeshPro text;

    #endregion

    #region Properties

    public string RelatedItemId { get { return relatedItemId; } set { relatedItemId = value; } }

    #endregion

    private string relatedItemId = "signpost";  //Id of the item used when placing this sign/to be dropped when destroying it

    protected override void Start()
    {
        base.Start();

        //Tell the WorldSave that this is a player-placed object that should be saved with the world
        WorldSave.Instance.AddPlacedObjectToSave(this);
    }

    private void OnDestroy()
    {
        //This object no longer exists in the world, remove it from the save list
        WorldSave.Instance.RemovePlacedObjectFromSaveList(this);
    }

    public void SetRelatedItem(string itemId)
    {
        relatedItemId = itemId;

        Item item = ItemManager.Instance.GetItemWithID(itemId);

        //Set sign text based on the related item's player-set properties
        SetSignText(    item.GetCustomStringPropertyWithName("line1").Value,
                        item.GetCustomStringPropertyWithName("line2").Value,
                        item.GetCustomStringPropertyWithName("line3").Value,
                        item.GetCustomStringPropertyWithName("line4").Value);
    }

    public void SetSignText(string line1, string line2, string line3, string line4)
    {
        //Sets multiple lines of text on the sign
        text.text = line1 + "\n" + line2 + "\n" + line3 + "\n" + line4;
    }

    public override void Destroyed()
    {
        base.Destroyed();
        Destroy(gameObject);

        //Get the player's inventory panel
        InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();

        //Give the player back the item used to placed the sign, which includes custom text properties
        inventory.AddItemToInventory(relatedItemId);
    }

    public void AddDataToWorldSave(SaveData saveData)
    {
        //Save the position and rotation of the sign in the world, as well as the id of the item used to place the sign
        saveData.AddData("sign*",   new SignpostSaveData()
                                    {
                                        Position = new float[3] { transform.position.x, transform.position.y, transform.position.z },
                                        Rotation = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z },
                                        RelatedItemId = RelatedItemId
                                    });
    }
}

//SignpostSaveData contains data used for saving/loading signposts

[System.Serializable]
public class SignpostSaveData
{
    public float[]  Position;
    public float[]  Rotation;
    public string   RelatedItemId;
}