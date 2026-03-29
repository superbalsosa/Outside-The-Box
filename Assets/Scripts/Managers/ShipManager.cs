using DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

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
    #endregion

    #region UNITY_METHODS
    private void Awake()
    {
        InterfaceDependencyInjector.Instance.Register<IShipManager>(() => this);
    }

    private void Start()
    {
        _eventSystem = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        InitShip();
        //To remove after logic is implemented
        Accelerate(100f, true);

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
    #endregion
}
