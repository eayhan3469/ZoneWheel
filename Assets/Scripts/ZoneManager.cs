using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private ZoneSettings zoneSettings;
    [SerializeField] private WheelController wheelController;

    public int CurrentZoneIndex { get; private set; } = 1;

    private void Start()
    {
        LoadZone(CurrentZoneIndex);
    }

    public void LoadZone(int zoneIndex)
    {
        CurrentZoneIndex = zoneIndex;

        WheelData data = zoneSettings.GetWheelForZone(CurrentZoneIndex);
        ZoneType type = zoneSettings.GetZoneType(CurrentZoneIndex);

        wheelController.SetupWheel(data);
    }

    public void AdvanceToNextZone()
    {
        LoadZone(CurrentZoneIndex + 1);
    }
}
