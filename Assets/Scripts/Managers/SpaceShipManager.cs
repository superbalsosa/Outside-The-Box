using Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpaceShipManager : Singleton<SpaceShipManager>
{
    public int CurrentHealth { get; private set; } = 100;
    public int CurrentBatterys { get; private set; } = 6;

    [Header("Space Ship Settings")]
    [SerializeField] private int starDust = 0;
    [SerializeField] private int batterysAmount = 0;

    [Header("UI")]
    [SerializeField] private Text starDustText;
    [SerializeField] private Text starShipText;
    [SerializeField] private Text batterysText;

    private void Start()
    {
        UpdateTexts();
    }

    public void ChangeHealth(int Amount)
    {
        CurrentHealth += Amount;
        starShipText.text = $"Starship Health: {CurrentHealth}";
        CheckDefeatSettings();
    }
    public void ChangeBatterys(int amount)
    {
        batterysAmount -= amount;
        CurrentBatterys += amount;
        CheckDefeatSettings();
    }

    public void ChangeStarDust(int amount)
    {
        starDust += amount;
        starDustText.text = $"Star Dust: {starDust}";
    }   

    public void GrabBatterys(int amount)
    {
        batterysAmount += amount;
        batterysText.text = $"Batterys: {CurrentBatterys}";
    }
    public void CheckDefeatSettings()
    {
        if (CurrentHealth <= 0 || CurrentBatterys <= 0)
        {
            // Trigger defeat condition
            Debug.Log("Defeat! Space ship is destroyed or out of batterys.");
        }
    }

    private void UpdateTexts()
    {
        starShipText.text = $"Starship Health: {CurrentHealth}";
        starDustText.text= $"Star Dust: {starDust}";
        batterysText.text = $"Batterys: {CurrentBatterys}";
    }
}
