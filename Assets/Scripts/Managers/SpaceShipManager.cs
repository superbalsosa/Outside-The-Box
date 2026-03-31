using Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DependencyInjection;
using System;

public class SpaceShipManager : Singleton<SpaceShipManager>, ISpaceShipManager
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
    public int MaxEnemyCount = 10;
    public int EnemyCount = 0;

    protected override void Awake()
    {
        base.Awake();
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
        starShipText.text = $"Starship Health: {CurrentHealth}";
        CheckDefeatSettings();
    }
    public void ChangeBatterys(int amount)
    {
        batterysAmount -= amount;
        CurrentBatterysInUse += amount;
        CheckDefeatSettings();
    }

    public void ChangeStardust(int amount)
    {
        starDust += amount;
        starDustText.text = $"Star Dust: {starDust}";
    }

    public void GrabBatterys(int amount)
    {
        batterysAmount += amount;
        batterysText.text = $"Batteries: {batterysAmount}";
    }
    public void CheckDefeatSettings()
    {
        if (CurrentHealth <= 0 || CurrentBatterysInUse <= 0)
        {
            // Trigger defeat condition
            Debug.Log("Defeat! Space ship is destroyed or out of batterys.");
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
    private bool HasBattery()
    {
        var hasBattery = batterysAmount > 0;
        if (hasBattery) batterysAmount--;

        return hasBattery;
    }

    #region INTERFACE_IMPLEMENTATION

    float ISpaceShipManager.GetStardust() => starDust;

    float ISpaceShipManager.GetMaxHP() => MaxHealth;
    bool ISpaceShipManager.GetBattery() => HasBattery();

    float ISpaceShipManager.GetCurrentHp() => CurrentHealth;

    void ISpaceShipManager.Heal(int hpToHeal) => ChangeHealth(hpToHeal);

    void ISpaceShipManager.ConsumeStardust(int stardustToConsume) => ChangeStardust(-stardustToConsume);
    #endregion
}
