interface IEventSystem
{
    EventType GetCurrentEvent();
    void ClearEvent();
    void SetEvent(EventType eventToSet, bool isOn);
    void SetAndClearEvent(EventType eventToSet, bool isOn);
    void SubscribeToEvent(EventType eventToSuscribe, IListener listener);
    void UnSubscribeToEvent(EventType eventToSuscribe, IListener listener);
}
