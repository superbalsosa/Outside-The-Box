using DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;


public class EventSystem : Singleton<EventSystem>, IEventSystem
{
    [System.Serializable]
    public class GameEvent
    {
        public GameEventSO gameEvent;
        public float delay;
        public int weight;
        public UnityEvent onEventTriggered;
    }
    [System.Serializable]
    public class InteractableEvent
    {
        public EventType eventType;
        public Action<bool> eventAction;
    }

    [SerializeField] private List<GameEvent> gameEvents = new List<GameEvent>();
    [SerializeField] private List<InteractableEvent> interactEvents = new List<InteractableEvent>();

    private GameEventSO currentEvent;
    private EventType currentInteractableEvent;
    protected override void Awake()
    {
        base.Awake();
        InterfaceDependencyInjector.Instance.Register<IEventSystem>(() => this);
    }


#if UNITY_EDITOR
    float delay = 10;
    void Update()
    {
        if (GetCurrentGameEvent() == null)
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
            {
                delay = 10;
                if (gameEvents.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, gameEvents.Count);
                    StartCoroutine(TriggerEvent(gameEvents[randomIndex]));
                }
            }
        }
    }
#endif
    private IEnumerator TriggerEvent(GameEvent e)
    {
        SetCurrentGameEvent(e.gameEvent);
        yield return new WaitForSeconds(e.delay);
        e.onEventTriggered.Invoke();
    }

    private IEnumerator TriggerRandomEventByTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 15f));
            if (gameEvents.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, gameEvents.Count);
                StartCoroutine(TriggerEvent(gameEvents[randomIndex]));
                SetCurrentGameEvent(gameEvents[randomIndex].gameEvent);
            }
        }
    }

    private IEnumerator TriggerRandomEventByWeight() 
    {         
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 15f));
            if (gameEvents.Count > 0)
            {
                int totalWeight = 0;
                foreach (var e in gameEvents)
                {
                    totalWeight += e.weight;
                }
                int randomWeight = UnityEngine.Random.Range(0, totalWeight);
                int currentWeight = 0;
                foreach (var e in gameEvents)
                {
                    currentWeight += e.weight;
                    if (randomWeight < currentWeight)
                    {
                        StartCoroutine(TriggerEvent(e));
                        SetCurrentGameEvent(e.gameEvent);
                        break;
                    }
                }
            }
        }
    }

    public GameEventSO GetCurrentGameEvent()
    {
        return currentEvent;
    }

    public void SetCurrentGameEvent(GameEventSO gameEvent)
    {
        currentEvent = gameEvent;
    }

    #region INTERFACE_METHODS
    EventType IEventSystem.GetCurrentEvent()
    {
        return currentInteractableEvent;
    }
    void IEventSystem.ClearEvent()
    {
        currentInteractableEvent = EventType.None;
        interactEvents.Find(e => e.eventType == EventType.None)?.eventAction?.Invoke(true);
    }
    void IEventSystem.SetEvent(EventType eventType, bool isOn)
    {
        currentInteractableEvent = eventType;
        interactEvents.Find(e => e.eventType == eventType)?.eventAction.Invoke(isOn);
    }
    void IEventSystem.SuscribeToEvent(EventType eventType, IListener listener)
    {
        interactEvents.Find(e => e.eventType == eventType).eventAction += listener.ExecuteListenerAction;
    }
    void IEventSystem.UnSuscribeToEvent(EventType eventType, IListener listener)
    {
        interactEvents.Find(e => e.eventType == eventType).eventAction -= (listener.ExecuteListenerAction);
    }
    #endregion 
}
