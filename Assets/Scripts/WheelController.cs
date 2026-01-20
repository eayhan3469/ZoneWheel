using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelController : MonoBehaviour
{
    [SerializeField] private Transform wheelRotator;
    [SerializeField] private Image wheelBackgroundImage;
    [SerializeField] private Image wheelIndicatorImage;

    [Header("Animation Settings")]
    [SerializeField] private float spinDuration = 4f;
    [SerializeField] private int minSpins = 5;
    [SerializeField] private Ease spinEase = Ease.OutQuart;

    private List<WheelSlot> _activeSlots = new List<WheelSlot>();
    private float _anglePerSlot;

    public bool IsSpinning { get; private set; } = false;

    public void SetupWheel(WheelData wheelData)
    {
        wheelBackgroundImage.sprite = wheelData.WheelBackground;
        wheelIndicatorImage.sprite = wheelData.WheelIndicator;

        WheelSlot[] existingSlots = wheelRotator.GetComponentsInChildren<WheelSlot>(true);


        if (existingSlots.Length == 0)
        {
            Debug.LogError("Couldn't found any 'WheelSlot'");
            return;
        }

        _activeSlots.Clear();

        if (wheelData.WheelEntries.Count != existingSlots.Length)
        {
            Debug.LogError($"Wheel Data entries count ({wheelData.WheelEntries.Count}) does not match the existing slots count ({existingSlots.Length})");
            return;
        }

        for (int i = 0; i < existingSlots.Length; i++)
        {
            WheelSlot slot = existingSlots[i];
            WheelEntry data = wheelData.WheelEntries[i];

            slot.gameObject.SetActive(true);
            slot.Setup(data);
            _activeSlots.Add(slot);
        }

        if (existingSlots.Length > 0)
            _anglePerSlot = 360f / existingSlots.Length;

        wheelRotator.rotation = Quaternion.identity;
    }

    public void SpinTo(int targetIndex, Action<WheelEntry> onSpinComplete)
    {
        if (IsSpinning)
            return;

        IsSpinning = true;

        // First slot is at the top (0 degreess)
        float targetAngle = -(targetIndex * _anglePerSlot);
        float finalRotation = targetAngle - (360f * minSpins);

        wheelRotator
            .DORotate(new Vector3(0, 0, finalRotation), spinDuration, RotateMode.FastBeyond360)
            .SetEase(spinEase)
            .OnComplete(() =>
            {
                IsSpinning = false;

                if (targetIndex >= 0 && targetIndex < _activeSlots.Count)
                {
                    WheelEntry winningEntry = _activeSlots[targetIndex].EntryData;
                    onSpinComplete?.Invoke(winningEntry);
                }
                else
                {
                    Debug.LogError($"Target index {targetIndex} is out of range of active slots {_activeSlots.Count}");
                    onSpinComplete?.Invoke(null);
                }
            });
    }
}
