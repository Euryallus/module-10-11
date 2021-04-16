using UnityEngine;
using TMPro;

public class Signpost : DestructableObject, IPersistentPlacedObject
{
    public string RelatedItemId { get { return relatedItemId; } set { relatedItemId = value; } }

    [SerializeField] private TextMeshPro text;

    private string relatedItemId = "signpost";

    protected override void Start()
    {
        base.Start();

        WorldSave.Instance.AddPlacedObjectToSave(this);
    }

    private void OnDestroy()
    {
        WorldSave.Instance.RemovePlacedObjectFromSaveList(this);
    }

    public void SetRelatedItem(string itemId)
    {
        relatedItemId = itemId;

        Item item = ItemManager.Instance.GetItemWithID(itemId);

        SetSignText(    item.GetCustomStringPropertyWithName("line1").Value,
                        item.GetCustomStringPropertyWithName("line2").Value,
                        item.GetCustomStringPropertyWithName("line3").Value,
                        item.GetCustomStringPropertyWithName("line4").Value);
    }

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

    public void AddDataToWorldSave(SaveData saveData)//, ref int uniqueId)
    {
        saveData.AddData("sign",    new SignpostSaveData()
                                    {
                                        Position = new float[3] { transform.position.x, transform.position.y, transform.position.z },
                                        Rotation = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z },
                                        RelatedItemId = RelatedItemId
                                    });
        //uniqueId++;
    }
}

[System.Serializable]
public class SignpostSaveData
{
    public float[]  Position;
    public float[]  Rotation;
    public string   RelatedItemId;
}