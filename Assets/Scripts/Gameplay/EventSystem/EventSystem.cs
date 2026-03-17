using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;


public class EventSystem : Singleton<EventSystem>
{
    [System.Serializable]
    public class GameEvent
    {
        public GameEventSO gameEvent;
        public float delay;
        public UnityEvent onEventTriggered;
    }

    [SerializeField] private List<GameEvent> events = new List<GameEvent>();
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TriggerEvent(GameEvent e)
    {
        yield return new WaitForSeconds(e.delay);
        e.onEventTriggered.Invoke();
    }
}
