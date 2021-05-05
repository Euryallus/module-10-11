public class CustomisingTable : PlaceableDestructible
{
    public override void AddDataToWorldSave(SaveData saveData)
    {
        base.AddDataToWorldSave(saveData);

        //Save the position, rotation and type of this object in the world
        saveData.AddData("customisingTable*",   new TransformSaveData()
                                                {
                                                    Position = new float[3] { transform.position.x, transform.position.y, transform.position.z },
                                                    Rotation = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z },
                                                });
    }

    protected override void DestroyedByPlayer()
    {
        base.DestroyedByPlayer();

        Destroy(gameObject);
    }
}