using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneUIManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ZoneManager zoneManager;

    [Header("Buttons")]
    [SerializeField] private Button spinButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button reviveConfirmButton;
    [SerializeField] private Button giveUpButton;

    [Header("Panels")]
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private UIGameOverPanel winPanel;

    private void OnEnable()
    {
        zoneManager.OnSpinStarted += HandleSpinStarted;
        zoneManager.OnZoneLoaded += HandleZoneLoaded;
        zoneManager.OnGameOver += HandleGameOver;
        zoneManager.OnGameWin += HandleGameWin;

        spinButton.onClick.AddListener(() => zoneManager.RequestSpin());
        exitButton.onClick.AddListener(() => zoneManager.RequestExit());

        reviveConfirmButton.onClick.AddListener(HandleReviveClick);
        giveUpButton.onClick.AddListener(HandleGiveUpClick);
    }

    private void OnDisable()
    {
        zoneManager.OnSpinStarted -= HandleSpinStarted;
        zoneManager.OnZoneLoaded -= HandleZoneLoaded;
        zoneManager.OnGameOver -= HandleGameOver;
        zoneManager.OnGameWin -= HandleGameWin;

        spinButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        reviveConfirmButton.onClick.RemoveAllListeners();
        giveUpButton.onClick.RemoveAllListeners();
    }

    private void HandleSpinStarted()
    {
        spinButton.interactable = false;
        exitButton.interactable = false;
    }

    private void HandleZoneLoaded(bool canExit)
    {
        spinButton.interactable = true;
        exitButton.interactable = canExit;

        revivePanel.SetActive(false);
    }

    private void HandleGameWin(List<CollectedItem> items)
    {
        winPanel.Show(items, () =>
        {
            zoneManager.ConfirmWin();
        });
    }

    private void HandleGameOver()
    {
        revivePanel.SetActive(true);
    }

    private void HandleReviveClick()
    {
        revivePanel.SetActive(false);
        zoneManager.RequestRevive();
    }

    private void HandleGiveUpClick()
    {
        revivePanel.SetActive(false);
        zoneManager.RequestGiveUp();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 1. Find ZoneManager (First look in children/parent, then global fallback)
        if (zoneManager == null)
        {
            zoneManager = GetComponentInChildren<ZoneManager>();

            if (zoneManager == null) 
                zoneManager = FindObjectOfType<ZoneManager>();
        }

        // 2. Find Buttons (Search ONLY in children)
        if (spinButton == null)
            spinButton = FindButtonInChildrenByName("UI_Button_Spin");

        if (exitButton == null)
            exitButton = FindButtonInChildrenByName("UI_Button_Exit");

        // Revive Panel Buttons
        if (reviveConfirmButton == null)
            reviveConfirmButton = FindButtonInChildrenByName("UI_Button_ReviveConfirm");

        if (giveUpButton == null)
            giveUpButton = FindButtonInChildrenByName("UI_Button_GiveUp");

        // 3. Find Panels (Search ONLY in children)
        if (revivePanel == null)
            revivePanel = FindObjectInChildrenByName("UI_Canvas_Panel_Revive");

        // Find Win Panel Script
        if (winPanel == null)
            winPanel = GetComponentInChildren<UIGameOverPanel>(true);
    }

    /// <summary>
    /// Searches for a Button component with the specific name within the children of this GameObject.
    /// </summary>
    private Button FindButtonInChildrenByName(string buttonName)
    {
        // Get all buttons in children, including inactive ones
        Button[] childButtons = GetComponentsInChildren<Button>(true);

        foreach (var btn in childButtons)
        {
            if (btn.name == buttonName)
            {
                return btn;
            }
        }

        // Requested Error: Log if not found in children
        Debug.LogError($"[UIManager] Error: Button named '{buttonName}' not found in children of '{gameObject.name}'.");
        return null;
    }

    /// <summary>
    /// Searches for a GameObject with the specific name within the children of this GameObject.
    /// </summary>
    private GameObject FindObjectInChildrenByName(string objName)
    {
        // Get all transforms in children to find the GameObject by name
        Transform[] children = GetComponentsInChildren<Transform>(true);

        foreach (var child in children)
        {
            if (child.name == objName)
            {
                return child.gameObject;
            }
        }

        Debug.LogError($"[UIManager] Error: GameObject named '{objName}' not found in children of '{gameObject.name}'.");
        return null;
    }
#endif
} // End of Class
