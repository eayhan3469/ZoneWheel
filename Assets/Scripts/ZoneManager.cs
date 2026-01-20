using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private ZoneSettings zoneSettings;
    [SerializeField] private WheelController wheelController;

    private WheelData _currentWheelData;

    public int CurrentZoneIndex { get; private set; } = 1;

    private void Start()
    {
        LoadZone(CurrentZoneIndex);
    }

    public void LoadZone(int zoneIndex)
    {
        CurrentZoneIndex = zoneIndex;
        _currentWheelData = zoneSettings.GetWheelForZone(CurrentZoneIndex);

        wheelController.SetupWheel(_currentWheelData);
    }

    public void AdvanceToNextZone()
    {
        LoadZone(CurrentZoneIndex + 1);
    }

    public void OnSpinButtonClicked()
    {
        if (wheelController.IsSpinning)
            return;

        int totalSlots = _currentWheelData.WheelEntries.Count;
        int targetIndex = Random.Range(0, totalSlots); //TODO: Implement weighted random based on DropChance

        wheelController.SpinTo(targetIndex, (earnedReward) =>
        {
            HandleSpinResult(earnedReward);
        });
    }

    private void HandleSpinResult(WheelEntry reward)
    {
        Debug.Log($"Sonuç Geldi: {reward.ItemData.DisplayName} x{reward.Amount}");

        if (reward.ItemData.Category == ItemCategory.Bomb)
        {
            Debug.Log("<color=red>BOMB!. Game Over!</color>");
            // TODO: Game Over UI aç
        }
        else
        {
            Debug.Log("<color=green>Congrats.</color>");
            // TODO: Ödülü envantere ekle

            AdvanceToNextZone();
        }
    }
}
