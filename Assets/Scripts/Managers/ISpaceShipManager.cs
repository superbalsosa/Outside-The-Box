interface ISpaceShipManager
{
    bool GetBattery();
    int GetEnemyCount();
    int GetMaxEnemyCount();
    bool GetDefeatStatus();
    float GetStardust();
    float GetMaxHP();
    float GetCurrentHp();
    void Heal(int hpToHeal);
    void AddEnemyCount(int amount);
    void ConsumeStardust(int stardustToConsume);
    void GrabBatterys(int amount);
}