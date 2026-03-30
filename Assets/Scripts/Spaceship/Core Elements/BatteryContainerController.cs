using DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

public class BatteryContainerController : MonoBehaviour, IListener
{
    [SerializeField] private EventType _eventToRefill;
    [SerializeField] private int _maxBatteries;
    [SerializeField] private int _currentBatteries;
    [SerializeField] private float _secondsPerBattery;

    private IEventSystem _eventManager;
    private ISpaceShipManager _spaceShipManager;
    private IShipManager _shipManager;
    private float _speedPercentagePerBattery;
    private bool _isUsingBattteries = false;

    void Start()
    {
        _shipManager = InterfaceDependencyInjector.Instance.Resolve<IShipManager>();
        _spaceShipManager = InterfaceDependencyInjector.Instance.Resolve<ISpaceShipManager>();
        InitEvents();
        _speedPercentagePerBattery = 100f/ _maxBatteries;
        _shipManager.SetSpeedPercentageTarget(_speedPercentagePerBattery * _currentBatteries);
    }

    void Update()
    {
        if (!_isUsingBattteries && _currentBatteries > 0)
        {
            _isUsingBattteries = true;
            StartCoroutine(ConsumeBatteryPerTime(_secondsPerBattery));
        }
    }

    private IEnumerator ConsumeBatteryPerTime(float secondsPerBattery)
    {
        yield return new WaitForSeconds(secondsPerBattery);
        _currentBatteries--;
        _shipManager.SetSpeedPercentageTarget(_speedPercentagePerBattery * _currentBatteries);
        _isUsingBattteries = false;
    }

    private void InitEvents()
    {
        _eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        _eventManager.SuscribeToEvent(_eventToRefill, this);
    }
    private bool BatterySlotAvailable() => _currentBatteries < _maxBatteries;

    private void AddBattery()
    {
        _currentBatteries++;
        _shipManager.SetSpeedPercentageTarget(_speedPercentagePerBattery * _currentBatteries);
    }

    #region INTERFACE_IMPLEMENTATION
    void IListener.ExecuteListenerAction(bool isOn)
    {
        if (_eventManager.GetCurrentEvent().Equals(_eventToRefill) && 
            isOn && 
            BatterySlotAvailable() &&
            _spaceShipManager.GetBattery()
            )
        {
            AddBattery();
        }
    }
    #endregion
}
