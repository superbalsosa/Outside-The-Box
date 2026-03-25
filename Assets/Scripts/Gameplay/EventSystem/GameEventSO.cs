using UnityEngine;

[CreateAssetMenu(menuName = "Events/Game Event")]

public class GameEventSO : ScriptableObject
{
    public EventType eventType;
    public string eventName;
}
