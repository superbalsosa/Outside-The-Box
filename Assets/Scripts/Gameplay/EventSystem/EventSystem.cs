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

    private IEnumerator TriggerRandomEventByTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            if (events.Count > 0)
            {
                int randomIndex = Random.Range(0, events.Count);
                StartCoroutine(TriggerEvent(events[randomIndex]));
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
                        break;
                    }
                }
            }
        }
    }
}
