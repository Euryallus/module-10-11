// ||=======================================================================||
// || ItemGroup: Defines a collection of items of the same type             ||
// ||=======================================================================||
// || Written by Joe for Module 10                                          ||
// ||=======================================================================||

[System.Serializable]
public class ItemGroup
{
    public ItemGroup(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public Item Item;
    public int Quantity;
}