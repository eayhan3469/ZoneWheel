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
    Currency = 0,
    Weapon = 1,
    Armor = 2,
    Consumable = 3,
    Points = 4,
    Chest = 5,
    Bomb = 6,
    Cosmetic = 7
}