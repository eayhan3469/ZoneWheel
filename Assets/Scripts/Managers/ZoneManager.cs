using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RunManager))]
public class ZoneManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ZoneSettings zoneConfig;

    [Header("System References")]
    [SerializeField] private WheelController wheelController;
    [SerializeField] private TapeController tapeController;

    [Header("Visual Feedback")]
    [SerializeField] private RewardAnimator rewardAnimator;

    public event Action OnSpinStarted;
    public event Action<bool> OnZoneLoaded;
    public event Action<WheelEntry> OnRewardEarned;
    public event Action OnGameOver;
    public event Action<List<CollectedItem>> OnGameWin;

    private RunManager _runManager;
    private WheelData _currentWheelData;
    private int _currentZoneIndex = 1;
    private bool _isAnimating = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        _runManager = GetComponent<RunManager>();

        InitializeGame();
    }

    public void InitializeGame()
    {
        _currentZoneIndex = 1;
        _runManager.StartNewRun();
        tapeController.Initialize(1, zoneConfig.TotalZones);
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
        OnZoneLoaded?.Invoke(canExit);
    }

    public void RequestSpin()
    {
        if (wheelController.IsSpinning || _isAnimating)
            return;

        _isAnimating = true;

        OnSpinStarted?.Invoke();

        int winnerIndex = wheelController.Random.Next();
        wheelController.SpinTo(winnerIndex, OnSpinCompleted);
    }

    public void RequestExit()
    {
        if (wheelController.IsSpinning)
            return;

        var collectedItems = _runManager.Stash.GetItems();
        OnGameWin?.Invoke(collectedItems);
    }

    public void RequestRevive()
    {
        StartCoroutine(WaitAndAdvanceRoutine(0.5f));
    }

    public void RequestGiveUp()
    {
        _runManager.GiveUp();
        ResetZone();
    }

    public void ConfirmWin()
    {
        _runManager.StartNewRun();
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
                _runManager.HandleReward(result);
                OnRewardEarned?.Invoke(result);
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
        OnGameOver?.Invoke();
    }

    private IEnumerator WaitAndAdvanceRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        LoadZone(_currentZoneIndex + 1);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (wheelController == null)
        {
            wheelController = FindObjectOfType<WheelController>();
            if (wheelController == null)
                Debug.LogError($"[ZoneManager] Error: WheelController not assigned and not found in scene.");
        }

        if(tapeController == null)
        {
            tapeController = FindObjectOfType<TapeController>();
            if (tapeController == null)
                Debug.LogError($"[ZoneManager] Error: TapeController not assigned and not found in scene.");
        }

        if (rewardAnimator == null)
        {
            rewardAnimator = FindObjectOfType<RewardAnimator>();
            if (rewardAnimator == null)
                Debug.LogError($"[ZoneManager] Error: RewardAnimator not assigned and not found in scene.");
        }
    }
#endif
}
