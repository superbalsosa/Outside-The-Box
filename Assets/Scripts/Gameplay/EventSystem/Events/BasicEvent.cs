using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class BasicEvent : Singleton<BasicEvent>
{ 
    [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();

    [SerializeField] private BasicEventObject BrokenShipPrefab;

    [SerializeField] public int ObjectsToSpawn = 1;

    public UnityEvent OnEventCompleted { get; private set; } = new UnityEvent();

    private void OnEnable()
    {
        OnEventCompleted.AddListener(CheckEventCompletion);
    }

    private void OnDisable()
    {
        OnEventCompleted.RemoveListener(CheckEventCompletion);
    }

    public void StartEvent (int AmountToSpawn)
    {
        ObjectsToSpawn = AmountToSpawn;
        for (int i = 0; i < ObjectsToSpawn; i++)
        {
            Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
            Instantiate(BrokenShipPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void CheckEventCompletion()
    {
        if (ObjectsToSpawn <= 0)
        {
            EventSystem.Instance.SetCurrentGameEvent(null);
        }
    }

    public void RemoveObjectFromEvent()
    {
        ObjectsToSpawn--;
    }

}

public interface IBasicEvent
{
    UnityEvent OnEventCompleted { get; }

    void RemoveObjectFromEvent();
}
