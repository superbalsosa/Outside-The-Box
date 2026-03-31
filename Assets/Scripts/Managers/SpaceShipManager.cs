using Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DependencyInjection;
using System;

public class SpaceShipManager : MonoBehaviour, ISpaceShipManager
{
    [Header("Space Ship Status")]
    [SerializeField] private int MaxHealth = 100;
    public int CurrentHealth { get; private set; } = 100;
    public int CurrentBatterysInUse { get; private set; } = 6;
    public bool BattleModeActive { get; private set; } = false;

    [Header("Player Inventory Settings")]
    [SerializeField] private int starDust = 0;
    [SerializeField] private int batterysAmount = 0;

    [Header("UI")]
    [SerializeField] private Text starDustText;
    [SerializeField] private Text starShipText;
    [SerializeField] private Text batterysText;

    [Header("Enemy Settings")]
    public int MaxEnemyCount { get; set; } = 10;
    public int EnemyCount { get; set; } = 0;

    private void Awake()
    {
        InterfaceDependencyInjector.Instance.Register<ISpaceShipManager>(() => this);
    }
    private void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateTexts();
    }

    public void ChangeHealth(int Amount)
    {
        CurrentHealth += Amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        UpdateTexts();
        CheckDefeatSettings();
    }
    public void ChangeBatterys(int amount)
    {
        batterysAmount -= amount;
        CurrentBatterysInUse += amount;
        batterysText.text = $"Batteries: {batterysAmount}";
        CheckDefeatSettings();
    }

    public void ChangeStardust(int amount)
    {
        starDust += amount;
        UpdateTexts();
    }

    public void GrabBatterys(int amount)
    {
        batterysAmount += amount;
        UpdateTexts();
    }
    public void CheckDefeatSettings()
    {
        if (CurrentHealth <= 0 || CurrentBatterysInUse <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("Defeat! Space ship is destroyed or out of batterys.");
        }
    }

    public bool GetDefeatStatus() 
    {
        if (CurrentHealth <= 0 || CurrentBatterysInUse <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TriggerBattleMode()
    {
        BattleModeActive = BattleModeActive ? false : true;
    }

    private void UpdateTexts()
    {
        starShipText.text = $"Starship Health: {CurrentHealth}";
        starDustText.text = $"Star Dust: {starDust}";
        batterysText.text = $"Batteries: {batterysAmount}";
    }
    private bool GetBattery()
    {
        var hasBattery = batterysAmount > 0;
        if (hasBattery)
        {
            batterysAmount--;
            UpdateTexts();
        }

        return hasBattery;
    }

    public void AddEnemyCount(int amount)
    {
        EnemyCount += amount;
    }
    #region INTERFACE_IMPLEMENTATION

    float ISpaceShipManager.GetStardust() => starDust;

    float ISpaceShipManager.GetMaxHP() => MaxHealth;

    int ISpaceShipManager.GetEnemyCount() => EnemyCount;
    int ISpaceShipManager.GetMaxEnemyCount() => MaxEnemyCount;
    void ISpaceShipManager.AddEnemyCount(int amount) => AddEnemyCount(amount);

    bool ISpaceShipManager.GetBattery() => GetBattery();

    bool ISpaceShipManager.GetDefeatStatus() => GetDefeatStatus();

    float ISpaceShipManager.GetCurrentHp() => CurrentHealth;

    void ISpaceShipManager.Heal(int hpToHeal) => ChangeHealth(hpToHeal);
    void ISpaceShipManager.GrabBatterys(int amount) => GrabBatterys(amount);
    void ISpaceShipManager.ConsumeStardust(int stardustToConsume) => ChangeStardust(-stardustToConsume);
    #endregion
}
