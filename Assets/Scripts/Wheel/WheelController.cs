using DG.Tweening;
using Subtegral.WeightedRandom;
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
    public WeightedRandom<int> Random { get; private set; }

    public void SetupWheel(WheelData wheelData)
    {
        Random = new WeightedRandom<int>();

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
            Random.Add(i, data.DropChance);

            slot.gameObject.SetActive(true);
            slot.Setup(data);
            _activeSlots.Add(slot);
        }

        if (existingSlots.Length > 0)
            _anglePerSlot = 360f / existingSlots.Length;

        wheelRotator.rotation = Quaternion.identity;
    }

    public void SpinTo(int targetIndex, Action<WheelEntry, Transform> onSpinComplete)
    {
        if (IsSpinning)
            return;

        IsSpinning = true;

        // First slot is at the top (0 degreess)
        float targetAngle = targetIndex * _anglePerSlot;
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
                    Transform winningSlotTransform = _activeSlots[targetIndex].transform;

                    onSpinComplete?.Invoke(winningEntry, winningSlotTransform);
                }
                else
                {
                    Debug.LogError($"Target index {targetIndex} is out of range of active slots {_activeSlots.Count}");
                    onSpinComplete?.Invoke(null, null);
                }
            });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (wheelRotator == null)
        {
            var transforms = GetComponentsInChildren<Transform>(true);
            bool found = false;
            foreach (var t in transforms)
            {
                if (t.name == "UI_Wheel_Rotator")
                {
                    wheelRotator = t;
                    found = true;
                    break;
                }
            }

            if (!found) 
                Debug.LogError($"[WheelController] Error: Transform named 'WheelRotator' not found in children of '{gameObject.name}'.");
        }

        if (wheelBackgroundImage == null)
        {
            var images = GetComponentsInChildren<Image>(true);
            bool found = false;
            foreach (var img in images)
            {
                if (img.name == "UI_Wheel_Rotator")
                {
                    wheelBackgroundImage = img;
                    found = true;
                    break;
                }
            }
            if (!found) 
                Debug.LogError($"[WheelController] Error: Image named 'Wheel_BG' not found in children of '{gameObject.name}'.");
        }

        if (wheelIndicatorImage == null)
        {
            var images = GetComponentsInChildren<Image>(true);
            bool found = false;
            foreach (var img in images)
            {
                if (img.name == "UI_Image_Wheel_Indicator")
                {
                    wheelIndicatorImage = img;
                    found = true;
                    break;
                }
            }
            if (!found) 
                Debug.LogError($"[WheelController] Error: Image named 'Wheel_Indicator' not found in children of '{gameObject.name}'.");
        }
    }
#endif
}
