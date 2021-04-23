using UnityEngine;

[System.Serializable]
public enum ModularPieceType
{
    WoodFloor,
    WoodWall
}

public class ModularPiece : DestructableObject, IPersistentPlacedObject
{
    [Header("Modular Piece")]
    [SerializeField] private ModularPieceType pieceType;

    protected override void Start()
    {
        base.Start();

        WorldSave.Instance.AddPlacedObjectToSave(this);
    }

    private void OnDestroy()
    {
        WorldSave.Instance.RemovePlacedObjectFromSaveList(this);
    }

    public void AddDataToWorldSave(SaveData saveData)
    {
        saveData.AddData("modularPiece*", new ModularPieceSaveData()
        {
            Position = new float[3] { transform.position.x, transform.position.y, transform.position.z },
            Rotation = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z },
            PieceType = pieceType
        });
    }

    public override void Destroyed()
    {
        base.Destroyed();

        Destroy(gameObject);
    }
}

[System.Serializable]
public class ModularPieceSaveData
{
    public ModularPieceType PieceType;
    public float[]          Position;
    public float[]          Rotation;
}