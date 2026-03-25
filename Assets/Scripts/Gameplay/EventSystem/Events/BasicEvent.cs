using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class BasicEvent : Singleton<BasicEvent>
{ 
    [SerializeField] private List<Transform> SpawnPointsFloor = new List<Transform>();
    [SerializeField] private List<Transform> SpawnPointsWall = new List<Transform>();

    [SerializeField] private BasicEventObject AlienPrefab;
    [SerializeField] private BasicEventObject BrokenWallPrefab;

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

    public void StartEventAlien (int AmountToSpawn)
    {
        ObjectsToSpawn = Random.Range(0, AmountToSpawn);
        if (ObjectsToSpawn <= 0) ObjectsToSpawn = 1;
        for (int i = 0; i < ObjectsToSpawn; i++)
        {
            Transform spawnPoint = SpawnPointsFloor[Random.Range(0, SpawnPointsFloor.Count)];
            Instantiate(AlienPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void StartEventBrokenWall(int AmountToSpawn)
    {
        ObjectsToSpawn = AmountToSpawn;
        for (int i = 0; i < ObjectsToSpawn; i++)
        {
            Transform spawnPoint = SpawnPointsWall[Random.Range(0, SpawnPointsWall.Count)];
            Instantiate(BrokenWallPrefab, spawnPoint.position, spawnPoint.rotation);
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
