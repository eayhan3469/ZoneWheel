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

    [Header("Visual Feedback")]
    [SerializeField] private RewardAnimator rewardAnimator;

    private WheelData _currentWheelData;
    private int _currentZoneIndex = 1;
    private bool _isGameActive = false;
    private bool _isAnimating = false;


    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        _currentZoneIndex = 1;
        _isGameActive = true;

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

        if (tapeController)
            tapeController.ScrollToZone(_currentZoneIndex);

        bool canExit = (zoneType == ZoneType.Safe || zoneType == ZoneType.Super);

        if (uiExitButton)
            uiExitButton.interactable = canExit;

        Debug.Log($"--- ZONE {_currentZoneIndex} BAŞLADI ({zoneType}) ---");
    }

    public void OnSpinButtonClicked()
    {
        if (!_isGameActive || wheelController.IsSpinning || _isAnimating)
            return;

        _isAnimating = true;
        uiExitButton.interactable = false;

        //TODO : Add weightedRandom
        int winnerIndex = wheelController.Random.Next();

        wheelController.SpinTo(winnerIndex, OnSpinCompleted);
    }

    public void OnExitButtonClicked()
    {
        if (!_isGameActive || wheelController.IsSpinning)
            return;

        Debug.Log("Leaving from run");
        runManager.CashOut();

        _isGameActive = false;

        ResetZone();
    }

    public void OnReviveButtonClicked()
    {
        Debug.Log("Revived!");

        uiRevivePanel.SetActive(false);
        _isGameActive = true;

        StartCoroutine(WaitAndAdvanceRoutine(0.5f));
    }

    public void OnGiveUpButtonClicked()
    {
        Debug.Log("Given up!");
        uiRevivePanel.SetActive(false);

        runManager.GiveUp();
        _isGameActive = false;

        ResetZone();
    }

    private void ResetZone()
    {
        _currentZoneIndex = 1;
        LoadZone(_currentZoneIndex);
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
                _isAnimating = false;
            });
        }
    }

    private void HandleSuccess()
    {
        // Başarılı ses efekti vs.
        StartCoroutine(WaitAndAdvanceRoutine(1.0f));
    }

    private void HandleGameOver()
    {
        Debug.Log("<color=red>GAME OVER!</color>");
        _isGameActive = false;

        if (uiRevivePanel)
            uiRevivePanel.SetActive(true);
    }

    private IEnumerator WaitAndAdvanceRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        LoadZone(_currentZoneIndex + 1);
    }
}
