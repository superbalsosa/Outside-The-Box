interface ISpaceShipManager
{
    bool GetBattery();
    float GetStardust();
    float GetMaxHP();
    float GetCurrentHp();
    void Heal(int hpToHeal);
    void ConsumeStardust(int stardustToConsume);
}