using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
using UnityEngine;

public class StarsContainerController : MonoBehaviour, IListener
{
    #region VARIABLES
    [SerializeField] private EventType _eventToRepair;
    [SerializeField] private int _stardustToRepair;
    [SerializeField] private int _spaceshipRepairHP;
    [SerializeField] private SoundData _repairSound;
    [SerializeField] private SoundData _cantRepairSound;

    private IEventSystem _eventManager;
    private ISoundManager _soundManager;
    private ISpaceShipManager _spaceShipManager;
    #endregion
    void Start()
    {
        InitEvents();
        _spaceShipManager = InterfaceDependencyInjector.Instance.Resolve<ISpaceShipManager>();
        _soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void InitEvents()
    {
        _eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        _eventManager.SubscribeToEvent(_eventToRepair, this);
    }
    private bool CanRepairShip()
    {
        bool canRepair = false;
        canRepair = _spaceShipManager.GetStardust() >=  _stardustToRepair &&
                       _spaceShipManager.GetMaxHP() > _spaceShipManager.GetCurrentHp() &&
                        _spaceShipManager.GetCurrentHp() > 0f;

        return canRepair;
    }
    private void RepairShip()
    {
        _spaceShipManager.ConsumeStardust(-_stardustToRepair);
        _spaceShipManager.Heal(_spaceshipRepairHP);
    }

    #region INTERFACE_IMPLEMENTATION
    void IListener.ExecuteListenerAction(bool isOn)
    {
        if (_eventManager.GetCurrentEvent().Equals(_eventToRepair) &&
            isOn &&
            CanRepairShip()
            )
        {
            _soundManager.CreateSound().WithSoundData(_repairSound).Play();
            RepairShip();
        }
        else
        {
            _soundManager.CreateSound().WithSoundData(_cantRepairSound).Play();
        }
        _eventManager.ClearEvent();
    }
    #endregion
}
