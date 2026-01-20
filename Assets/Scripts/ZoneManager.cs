using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ZoneSettings zoneSettings;

    [Header("Controllers")]
    [SerializeField] private WheelController wheelController;
    [SerializeField] private TapeController tapeController;

    private WheelData _currentWheelData;
    private int _currentZone;

    private void Start()
    {
        InitZone(1);
        tapeController.Initialize(1, zoneSettings.TotalZones);
    }

    private void InitZone(int zoneIndex)
    {
        _currentZone = zoneIndex;
        _currentWheelData = zoneSettings.GetWheelForZone(zoneIndex);

        wheelController.SetupWheel(_currentWheelData);
        tapeController.ScrollToZone(zoneIndex);
    }

    public void OnSpinButtonClicked()
    {
        if (wheelController.IsSpinning)
            return;

        int winnerIndex = PickRandomIndex();

        wheelController.SpinTo(winnerIndex, (result) =>
        {
            OnSpinCompleted(result);
        });
    }

    private int PickRandomIndex()
    {
        //TODO: Implement weighted random based on DropChance
        return Random.Range(0, _currentWheelData.WheelEntries.Count);
    }

    private void OnSpinCompleted(WheelEntry result)
    {
        if (result == null)
            return;

        if (result.ItemData.Category == ItemCategory.Bomb)
        {
            Debug.Log("<color=red>BOMBA! Oyun Bitti.</color>");
            // TODO: Fail Screen
        }
        else
        {
            Debug.Log($"<color=green>KAZANDIN: {result.ItemData.DisplayName}</color>");
            // TODO: Inventory Add

            // Sonraki Level
            Invoke(nameof(NextZone), 1f); // Biraz bekle sonra geç
        }
    }

    private void NextZone()
    {
        InitZone(_currentZone + 1);
    }
}
