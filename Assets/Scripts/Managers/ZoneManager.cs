using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZoneManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ZoneSettings zoneConfig;

    [Header("System References")]
    [SerializeField] private WheelController wheelController;
    [SerializeField] private TapeController tapeController;
    [SerializeField] private RunManager runManager;

    [Header("UI References")]
    [SerializeField] private Button uiSpinButton;
    [SerializeField] private Button uiExitButton;
    [SerializeField] private GameObject uiRevivePanel;
    [SerializeField] private UIGameOverPanel uiWinPanel;

    [Header("Visual Feedback")]
    [SerializeField] private RewardAnimator rewardAnimator;

    private WheelData _currentWheelData;
    private int _currentZoneIndex = 1;
    private bool _isAnimating = false;


    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        _currentZoneIndex = 1;

        uiRevivePanel.SetActive(false);
        runManager.StartNewRun();
        tapeController.Initialize(_currentZoneIndex, zoneConfig.TotalZones);

        LoadZone(_currentZoneIndex);
    }

    private void LoadZone(int index)
    {
        _currentZoneIndex = index;

        _currentWheelData = zoneConfig.GetWheelForZone(_currentZoneIndex);
        ZoneType zoneType = zoneConfig.GetZoneType(_currentZoneIndex);

        wheelController.SetupWheel(_currentWheelData);

        tapeController.ScrollToZone(_currentZoneIndex, () => { _isAnimating = false; });

        bool canExit = (zoneType == ZoneType.Safe || zoneType == ZoneType.Super);

        if (uiExitButton)
            uiExitButton.interactable = canExit;
    }

    public void OnSpinButtonClicked()
    {
        if (wheelController.IsSpinning || _isAnimating)
            return;

        _isAnimating = true;
        uiExitButton.interactable = false;

        //TODO : Add weightedRandom
        int winnerIndex = wheelController.Random.Next();

        wheelController.SpinTo(winnerIndex, OnSpinCompleted);
    }

    public void OnExitButtonClicked()
    {
        if (wheelController.IsSpinning)
            return;

        var collectedItems = runManager.Stash.GetItems();

        uiWinPanel.Show(collectedItems, () =>
        {
            runManager.StartNewRun();

            ResetZone();
        });
    }

    public void OnReviveButtonClicked()
    {
        uiRevivePanel.SetActive(false);

        StartCoroutine(WaitAndAdvanceRoutine(0.5f));
    }

    public void OnGiveUpButtonClicked()
    {
        Debug.Log("Given up!");
        uiRevivePanel.SetActive(false);

        runManager.GiveUp();

        ResetZone();
    }

    private void ResetZone()
    {
        InitializeGame();
    }

    private void OnSpinCompleted(WheelEntry result, Transform slotTransform)
    {
        if (result == null)
            return;

        if (result.ItemData.Category == ItemCategory.Bomb)
        {
            HandleGameOver();
            _isAnimating = false;
        }
        else
        {
            rewardAnimator.PlayRewardAnimation(slotTransform.position, result.ItemData.Icon, () =>
            {
                runManager.HandleReward(result);
                HandleSuccess();
            });
        }
    }

    private void HandleSuccess()
    {
        StartCoroutine(WaitAndAdvanceRoutine(0.2f));
    }

    private void HandleGameOver()
    {
        if (uiRevivePanel)
            uiRevivePanel.SetActive(true);
    }

    private IEnumerator WaitAndAdvanceRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        LoadZone(_currentZoneIndex + 1);
    }
}
