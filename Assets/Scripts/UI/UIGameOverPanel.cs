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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (claimButton == null)
        {
            var buttons = GetComponentsInChildren<Button>(true);
            bool found = false;
            foreach (var btn in buttons)
            {
                if (btn.name == "UI_Button_Claim")
                {
                    claimButton = btn;
                    found = true;
                    break;
                }
            }
            if (!found) 
                Debug.LogError($"[UIGameOverPanel] Error: Button named 'Button_Claim' not found in children of '{gameObject.name}'.");
        }

        if (contentRect == null)
        {
            var rects = GetComponentsInChildren<RectTransform>(true);
            bool found = false;
            foreach (var rect in rects)
            {
                if (rect.name == "UI_Content")
                {
                    contentRect = rect;
                    found = true;
                    break;
                }
            }
            if (!found) 
                Debug.LogError($"[UIGameOverPanel] Error: RectTransform named 'Content' not found in children of '{gameObject.name}'.");
        }

        if (layoutGroup == null && contentRect != null)
        {
            layoutGroup = contentRect.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null) Debug.LogError($"[UIGameOverPanel] Error: 'HorizontalLayoutGroup' component missing on '{contentRect.name}'.");
        }

        // 4. Panel Object (Self)
        if (panelObject == null) panelObject = this.gameObject;
    }
#endif
}