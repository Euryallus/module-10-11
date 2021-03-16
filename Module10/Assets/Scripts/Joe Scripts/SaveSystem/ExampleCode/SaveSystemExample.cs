using System.Collections.Generic;

public class SaveSystemExample : PersistentObject
{

    private int exampleInt;

    private List<string> exampleList = new List<string>() { "thing1", "thing2", "thing3" };

    private float[] exampleArray = new float[] { 0.1f, 7.8f, 69.0f };

    public override void OnSave(SaveData saveData)
    {
        //OnLoad is called each time game data is saved to a file

        //This is where you add any values that you want to save to the save data

        //You can save any serializable data type. See this for a full list: https://docs.microsoft.com/en-us/dotnet/standard/serialization/binary-serialization

        //Example:

        saveData.AddData("intToSave", exampleInt);

        saveData.AddData("listToSave", exampleList);

        saveData.AddData("arrayToSave", exampleArray);
    }

    public override void OnLoad(SaveData saveData)
    {
        //OnLoad is called each time game data is loaded from a file

        //This is where you initialise an object with the loaded values

        //Example:

        exampleInt = saveData.GetData<int>("intToSave");

        exampleList = saveData.GetData<List<string>>("listToSave");

        exampleArray = saveData.GetData<float[]>("arrayToSave");
    }
}
