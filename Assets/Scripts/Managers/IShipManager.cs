public interface IShipManager
{
    void Accelerate(float percentageToAccelerate, bool isPositive);
    float GetCurrentMetersLeft();
    float GetCurrentSpeed();
    void SetSpeedPercentageTarget(float percentageTarget);
}