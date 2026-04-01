using Audio;
using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryContainerController : MonoBehaviour, IListener
{
    #region VARIABLES
    [SerializeField] private EventType _eventToRefill;
    [SerializeField] private int _maxBatteries;
    [SerializeField] private int _currentBatteries;
    [SerializeField] private float _secondsPerBattery;
    [SerializeField] private Material _bulbOnMaterial;
    [SerializeField] private Material _bulbOffMaterial;
    [SerializeField] private List<Renderer> _rendererBulbs;
    [SerializeField] private SoundData _refillSound;
    [SerializeField] private SoundData _cantRefillSound;
    [SerializeField] private SoundData _batteryConsumedSound;

    private IEventSystem _eventManager;
    private ISpaceShipManager _spaceShipManager;
    private IShipManager _shipManager;
    private ISoundManager _soundManager;
    private float _speedPercentagePerBattery;
    private bool _isUsingBattteries = false;
    #endregion

    #region UNITY_METHODS
    void Start()
    {
        _shipManager = InterfaceDependencyInjector.Instance.Resolve<IShipManager>();
        _spaceShipManager = InterfaceDependencyInjector.Instance.Resolve<ISpaceShipManager>();
        _soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();
        InitEvents();
        _speedPercentagePerBattery = 100f/ _maxBatteries;
        _shipManager.SetSpeedPercentageTarget(_speedPercentagePerBattery * _currentBatteries);
        SyncMaterials();
    }
    void Update()
    {
        if (!_isUsingBattteries && _currentBatteries > 0)
        {
            _isUsingBattteries = true;
            StartCoroutine(ConsumeBatteryPerTime(_secondsPerBattery));
        }
    }
    private void OnDisable()
    {
        _eventManager.UnSubscribeToEvent(_eventToRefill, this);
    }
    #endregion

    #region PRIVATE_METHODS
    private void SyncMaterials()
    {
        var currentOutline = _rendererBulbs[0].material.GetFloat("_OutlineWidth");
        for (int i = 0; i < _rendererBulbs.Count; i++)
        {
            if (i < _currentBatteries)
            {
                _rendererBulbs[i].material = _bulbOnMaterial;
            }
            else
            {
                _rendererBulbs[i].material = _bulbOffMaterial;
            }
            _rendererBulbs[i].material.SetFloat("_OutlineWidth", currentOutline);
        }
    }
    private IEnumerator ConsumeBatteryPerTime(float secondsPerBattery)
    {
        yield return new WaitForSeconds(secondsPerBattery);
        _soundManager.CreateSound().WithSoundData(_batteryConsumedSound).Play();
        _currentBatteries--;
        _shipManager.SetSpeedPercentageTarget(_speedPercentagePerBattery * _currentBatteries);
        _isUsingBattteries = false;
        SyncMaterials();
    }
    private void InitEvents()
    {
        _eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        _eventManager.SubscribeToEvent(_eventToRefill, this);
    }
    private bool BatterySlotAvailable() => _currentBatteries < _maxBatteries;
    private void AddBattery()
    {
        _currentBatteries++;
        _shipManager.SetSpeedPercentageTarget(_speedPercentagePerBattery * _currentBatteries);
        SyncMaterials();
    }
    #endregion

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
            _soundManager.CreateSound().WithSoundData(_refillSound).Play();
        }
        else
        {
            _soundManager.CreateSound().WithSoundData(_cantRefillSound).Play();
        }
        _eventManager.ClearEvent();
    }
    #endregion
}
