using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class TapeController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private RectTransform viewportRect;

    [Header("Prefab Settings")]
    [SerializeField] private ZoneNumberView zoneNumberPrefab;
    [SerializeField] private float itemWidth = 45f;

    private float _stepSize;

    public void Initialize(int startZone, int totalZones)
    {
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        float centerOffset = (viewportRect.rect.width / 2f) - (itemWidth / 2f);
        layoutGroup.padding.left = Mathf.RoundToInt(centerOffset);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        _stepSize = itemWidth + layoutGroup.spacing;

        for (int i = 1; i <= totalZones; i++)
        {
            var zoneView = Instantiate(zoneNumberPrefab, contentRect);

            if (i % 30 == 0)
                zoneView.Setup(i, Color.yellow); // Super Zone
            else if (i % 5 == 0)
                zoneView.Setup(i, Color.green); // Safe Zone
            else
                zoneView.Setup(i, Color.white);
        }

        ScrollToZone(startZone);
    }

    public void ScrollToZone(int zoneNumber, Action onComplete = null)
    {
        if (zoneNumber < 1)
            zoneNumber = 1;

        int targetIndex = zoneNumber - 1;
        float targetX = -1 * (targetIndex * _stepSize);

        contentRect.DOKill();
        contentRect.DOAnchorPosX(targetX, 0.25f).SetEase(Ease.OutBack).OnComplete(()=> onComplete?.Invoke());
    }
}
