using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOverPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private RectTransform viewportRect;

    [SerializeField] private Button claimButton;
    [SerializeField] private GameObject panelObject;

    [Header("Prefab Settings")]
    [SerializeField] private UIStashItem itemPrefab;
    [SerializeField] private float itemWidth = 100f;

    private Action _onConfirmAction;

    private void OnEnable()
    {
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    public void Show(List<CollectedItem> earnedItems, Action onConfirm)
    {
        _onConfirmAction = onConfirm;

        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        float centerOffset = (viewportRect.rect.width / 2f) - (itemWidth / 2f);
        layoutGroup.padding.left = Mathf.RoundToInt(centerOffset);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        foreach (var item in earnedItems)
        {
            UIStashItem uiItem = Instantiate(itemPrefab, contentRect);
            uiItem.Setup(item.Data.Icon, item.Amount);
        }

        panelObject.SetActive(true);
    }

    private void OnClaimClicked()
    {
        panelObject.SetActive(false);

        _onConfirmAction?.Invoke();
    }
}