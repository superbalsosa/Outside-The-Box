using DependencyInjection;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class BoxOpenerSpawner : MonoBehaviour, IBoxOpenerSpawner
{
    [SerializeField] private GameObject boxOpenerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    private Dictionary<Transform, GameObject> occupiedPoints = new Dictionary<Transform, GameObject>();

    public Action OnSpawnPointFreed { get; set; }

    private void Awake()
    {
        InterfaceDependencyInjector.Instance.Register<IBoxOpenerSpawner>(() => this);

        foreach (var point in spawnPoints)
        {
            occupiedPoints.Add(point, null);
        }
    }

    public void SpawnBox()
    {
        foreach (var point in spawnPoints)
        {
            if (occupiedPoints[point] == null) 
            {
                GameObject box = Instantiate(boxOpenerPrefab, point.position, Quaternion.identity);

                box.GetComponent<BoxOpener>().Init(this, point);

                occupiedPoints[point] = box;

                return; 
            }
        }

        Debug.Log("No hay spawn points libres");
    }

    public void FreeSpawnPoint(Transform point)
    {
        if (occupiedPoints.ContainsKey(point))
        {
            occupiedPoints[point] = null;
            OnSpawnPointFreed?.Invoke();
        }
    }

    public bool IsThereFreeSpawnPoints()
    {
        bool hasFreePoints = false;
        foreach (var point in spawnPoints)
        {
            if (occupiedPoints[point] == null) 
            {
                hasFreePoints = true;
            }
            else
            {
                continue;
            }
        }
        return hasFreePoints;
    }
}

public interface IBoxOpenerSpawner
{
    Action OnSpawnPointFreed { get; set; }
    public void SpawnBox();

    void FreeSpawnPoint(Transform point);

    bool IsThereFreeSpawnPoints();
}
