interface IEventSystem
{
    EventType GetCurrentEvent();
    void ClearEvent();
    void SetEvent(EventType eventToSet, bool isOn);
    void SetAndClearEvent(EventType eventToSet, bool isOn);
    void SuscribeToEvent(EventType eventToSuscribe, IListener listener);
    void UnSuscribeToEvent(EventType eventToSuscribe, IListener listener);
}
