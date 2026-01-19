using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DAT_Wheel_", menuName = "Game/WheelData", order = 1)]
public class WheelData : ScriptableObject
{
    public string WheelName;
    
    public List<WheelEntry> WheelEntries;
}

public enum WheelType : byte
{
    Bronze,
    Silver,
    Gold
}

[Serializable]
public class WheelEntry
{
    public ItemData ItemData;
    public int Amount;
    [Range(0, 100)] public int DropChance;
}
