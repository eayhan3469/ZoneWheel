using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class TapeController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private RectTransform viewportRect;

    [Header("Prefab Settings")]
    [SerializeField] private GameObject zoneNumberPrefab;
    [SerializeField] private float itemWidth = 45f;

    private float _stepSize;

    [Header("--- TEST CONTROLS ---")]
    [SerializeField] private int testTotalZones = 100;
    [SerializeField] private int testTargetZone = 5;

    private void Start()
    {
        Initialize(testTotalZones);
    }

    public void Initialize(int totalZones)
    {
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        float centerOffset = (viewportRect.rect.width / 2f) - (itemWidth / 2f);

        layoutGroup.padding.left = Mathf.RoundToInt(centerOffset);

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        _stepSize = itemWidth + layoutGroup.spacing;

        for (int i = 1; i <= totalZones; i++)
        {
            GameObject obj = Instantiate(zoneNumberPrefab, contentRect);

            TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (tmp != null)
            {
                tmp.text = i.ToString();

                if (i % 30 == 0)
                    tmp.color = Color.yellow; // Super Zone
                else if (i % 5 == 0)
                    tmp.color = Color.green; // Safe Zone
                else
                    tmp.color = Color.white;
            }
        }
    }

    public void ScrollToZone(int zoneNumber)
    {
        if (zoneNumber < 1) 
            zoneNumber = 1;

        int targetIndex = zoneNumber - 1;
        float targetX = -1 * (targetIndex * _stepSize);

        contentRect.DOKill();
        contentRect.DOAnchorPosX(targetX, 0.5f).SetEase(Ease.OutBack);

        Debug.Log($"<color=cyan>UI:</color> Zone {zoneNumber} noktasına kaydırıldı.");
    }

    #region TEST_METHODS

    [ContextMenu("Test: Scroll To Target")]
    public void Test_ScrollToTarget()
    {
        ScrollToZone(testTargetZone);
    }

    [ContextMenu("Test: Next Zone")]
    public void Test_NextZone()
    {
        testTargetZone++;
        ScrollToZone(testTargetZone);
    }
    #endregion
}