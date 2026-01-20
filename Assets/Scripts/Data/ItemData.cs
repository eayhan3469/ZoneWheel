using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DAT_Item_", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string DisplayName;
    public Sprite Icon;
    public ItemCategory Category;
}

public enum ItemCategory : byte
{
    Coin,
    Gold,
    Weapon,
    Armor,
    Bomb
}