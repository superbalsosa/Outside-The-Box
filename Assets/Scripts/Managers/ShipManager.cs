using DependencyInjection;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipManager : MonoBehaviour, IShipManager
{
    #region VARIABLES
    [SerializeField] private float totalMetersLevel;
    [SerializeField] private float metersPerSecondAtFullSpeed;
    [SerializeField, Range(0,100)] private float startingSpeedPercentage;
    [SerializeField, Range(0, 5)] private float secondsToReachTargetSpeed;

    private float _currentMetersLeft;
    private float _metersPerSecond;
    private float _speedPercentage;
    private float _secondsToReachTargetSpeed;
    private bool _isStopped;
    private IEventSystem _eventSystem;
    private IPauseMenuManager _pauseMenuManager;
    private ISpaceShipManager _spaceShipManager;

    [Header("UI End Level Settings")]
    [SerializeField] private TextMeshProUGUI EndLevelText;
    [SerializeField] private MenuPanel WinPanel;
    [SerializeField] private MenuPanel LosePanel;
    #endregion

    #region UNITY_METHODS
    private void Awake()
    {
        InitShip();
        InterfaceDependencyInjector.Instance.Register<IShipManager>(() => this);
    }

    private void Start()
    {
        _eventSystem = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        _pauseMenuManager = InterfaceDependencyInjector.Instance.Resolve<IPauseMenuManager>();
        _spaceShipManager = InterfaceDependencyInjector.Instance.Resolve<ISpaceShipManager>();
    }
    private void Update()
    {
        CheckShipState();
        MoveShip();
    }
    #endregion

    #region PRIVATE_METHODS
    private void InitShip()
    {
        _currentMetersLeft = totalMetersLevel;
        _speedPercentage = startingSpeedPercentage;
        _metersPerSecond = metersPerSecondAtFullSpeed;
        _secondsToReachTargetSpeed = secondsToReachTargetSpeed;
    }
    private void CheckShipState()
    {
        if (_isStopped && _speedPercentage > 0) { _isStopped = false; }
        if (!_isStopped && _speedPercentage <= 0) { _isStopped = true; }
        if (_isStopped && _currentMetersLeft <= 0) { _eventSystem.SetEvent(EventType.LevelCompleted, true);}
        if (_eventSystem.GetCurrentEvent().Equals(EventType.LevelCompleted)) { SetUIEndGame(true); }
        if (_spaceShipManager.GetDefeatStatus()) { SetUIEndGame(false); }

    }
    private void MoveShip()
    {
        if (_isStopped) return;

        _currentMetersLeft -= Time.deltaTime * _metersPerSecond * (_speedPercentage / 100f);

        if (_currentMetersLeft <= 0 && _speedPercentage != 0) {
            _currentMetersLeft = 0;
            Accelerate(_secondsToReachTargetSpeed, false);
        }
    }
    private void Accelerate(float percentageToAcelerate, bool isPositive)
    {
        percentageToAcelerate = Mathf.Abs(percentageToAcelerate);
        var sign = isPositive ? 1 : -1;
        var targetSpeedPercentage = Mathf.Clamp(_speedPercentage + percentageToAcelerate * sign, 0f, 100f);

        StartCoroutine(AccelerateDecelerateInSeconds(_speedPercentage, targetSpeedPercentage, _secondsToReachTargetSpeed));
    }
    private void AccelerateToTargetSpeed(float percentageTarget)
    {
        percentageTarget = Mathf.Abs(percentageTarget);
        var targetSpeedPercentage = Mathf.Clamp(percentageTarget, 0f, 100f);

        StartCoroutine(AccelerateDecelerateInSeconds(_speedPercentage, targetSpeedPercentage, _secondsToReachTargetSpeed));
    }
    private IEnumerator AccelerateDecelerateInSeconds(float startSpeedPercentage, float targetSpeedPercentage, float secondsToReachTarget)
    {
        float elapsed = 0f;

        while (elapsed < secondsToReachTarget)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / secondsToReachTarget;

            _speedPercentage = Mathf.Clamp(Mathf.Lerp(startSpeedPercentage, targetSpeedPercentage, t), 0f, 100f); 

            yield return null;
        }

        _speedPercentage = Mathf.Clamp(targetSpeedPercentage, 0f, 100f);
    }

    private void SetUIEndGame(bool didWin)
    {
        if (didWin)
        {
            _pauseMenuManager.DissableAllUI();
            WinPanel.gameObject.SetActive(true);
            EndLevelText.gameObject.SetActive(true);
            EndLevelText.text = WinPanel.panelTitle;
        }
        else
        {
            _pauseMenuManager.DissableAllUI();
            LosePanel.gameObject.SetActive(true);
            EndLevelText.gameObject.SetActive(true);
            EndLevelText.text = LosePanel.panelTitle;
        }
    }

    public void ContinueGame()
    {
        _eventSystem.SetEvent(EventType.LevelCompleted, false);
        _eventSystem.ClearEvent();
        _pauseMenuManager.EnableGameplayUI();
        WinPanel.gameObject.SetActive(false);
        EndLevelText.gameObject.SetActive(false);
        SaveBestMeterDistance();
        totalMetersLevel = totalMetersLevel + totalMetersLevel * 0.3f;
        InitShip();
    }

    public void ExitToMainMenu()
    {
        _eventSystem.SetEvent(EventType.LevelCompleted, false);
        _eventSystem.ClearEvent();
        SaveBestMeterDistance();
        LosePanel.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveBestMeterDistance()
    {
        float bestDistance = PlayerPrefs.GetFloat("BestDistance", 0f);
        float distanceTraveled = totalMetersLevel - _currentMetersLeft;
        if (distanceTraveled > bestDistance)
        {
            PlayerPrefs.SetFloat("BestDistance", distanceTraveled);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region INTERFACE_IMPLEMENTATION
    void IShipManager.Accelerate(float percentageToAccelerate, bool isPositive)
    {
        Accelerate(percentageToAccelerate, isPositive);
    }
    float IShipManager.GetCurrentMetersLeft()
    {
        return _currentMetersLeft;
    }
    float IShipManager.GetCurrentSpeed()
    {
        return _speedPercentage;
    }

    void IShipManager.SetSpeedPercentageTarget(float percentageTarget)
    {
        AccelerateToTargetSpeed(percentageTarget);
    }
    #endregion
}
