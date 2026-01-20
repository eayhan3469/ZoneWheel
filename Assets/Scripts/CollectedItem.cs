using System;

[Serializable]
public class CollectedItem
{
    public ItemData Data;
    public int Amount;

    public CollectedItem(ItemData data, int amount)
    {
        Data = data;
        Amount = amount;
    }
}
