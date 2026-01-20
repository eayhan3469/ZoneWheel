using UnityEngine;

[CreateAssetMenu(fileName = "DAT_ZoneSettings", menuName = "Game/ZoneSettings")]
public class ZoneSettings : ScriptableObject
{
    public int TotalZones = 100;

    public int SafeZoneInterval = 5;
    public int SuperZoneInterval = 30;

    public WheelData StandartWheel;
    public WheelData SafeZoneWheel;
    public WheelData SuperZoneWheel;

    public WheelData GetWheelForZone(int zoneIndex)
    {
        if (zoneIndex % SuperZoneInterval == 0)
            return SuperZoneWheel;

        if (zoneIndex % SafeZoneInterval == 0)
            return SafeZoneWheel;

        return StandartWheel;
    }

    public ZoneType GetZoneType(int zoneIndex)
    {
        if (zoneIndex % SuperZoneInterval == 0)
            return ZoneType.Super;

        if (zoneIndex % SafeZoneInterval == 0)
            return ZoneType.Safe;

        return ZoneType.Standard;
    }
}


public enum ZoneType
{
    Standard,
    Safe,
    Super
}