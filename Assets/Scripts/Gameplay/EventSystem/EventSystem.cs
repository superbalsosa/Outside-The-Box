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
        public int weight;
        public UnityEvent onEventTriggered;
    }

    [SerializeField] private List<GameEvent> events = new List<GameEvent>();
    
    private GameEventSO currentEvent;
    protected override void Awake()
    {
        base.Awake();
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
                if (events.Count > 0)
                {
                    int randomIndex = Random.Range(0, events.Count);
                    StartCoroutine(TriggerEvent(events[randomIndex]));
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
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            if (events.Count > 0)
            {
                int randomIndex = Random.Range(0, events.Count);
                StartCoroutine(TriggerEvent(events[randomIndex]));
                SetCurrentGameEvent(events[randomIndex].gameEvent);
            }
        }
    }

    private IEnumerator TriggerRandomEventByWeight() 
    {         
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            if (events.Count > 0)
            {
                int totalWeight = 0;
                foreach (var e in events)
                {
                    totalWeight += e.weight;
                }
                int randomWeight = Random.Range(0, totalWeight);
                int currentWeight = 0;
                foreach (var e in events)
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
}
