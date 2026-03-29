using DependencyInjection;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class BoxOpenerSpawner : MonoBehaviour, IBoxOpenerSpawner
{
    [SerializeField] private GameObject boxOpenerPrefab;
    [SerializeField] private List<BoxOpener> spawnPoints = new List<BoxOpener>();
    private Dictionary<Transform, GameObject> occupiedPoints = new Dictionary<Transform, GameObject>();

    public Action OnSpawnPointFreed { get; set; }

    private void Awake()
    {
        InterfaceDependencyInjector.Instance.Register<IBoxOpenerSpawner>(() => this);

        foreach (var point in spawnPoints)
        {
            occupiedPoints.Add(point.transform, null);
        }
    }

    public void SpawnBox()
    {
        foreach (var point in spawnPoints)
        {
            if (occupiedPoints[point.transform] == null)
            {
                //    GameObject box = ObjectPoolManager.SpawnSpaceObject(boxOpenerPrefab, point.transform.position, Quaternion.identity, ObjectPoolManager.PoolType.BoxeOpener);

                point.GetComponent<BoxOpener>().Init(this);

                point.gameObject.layer = LayerMask.NameToLayer("Interactable");

                point.BoxGameObject.gameObject.SetActive(true);

                occupiedPoints[point.transform] = point.BoxGameObject;

                return; 
            }
        }

        Debug.Log("No hay spawn points libres");
    }

    public void UnSpawnBox(BoxOpener box)
    {
        foreach (var point in spawnPoints)
        {
            if (occupiedPoints[point.transform] != null &&
                occupiedPoints[point.transform] == box.gameObject)
            {

                point.BoxGameObject.SetActive(false);

                occupiedPoints[point.transform] = null;

                return;
            }
        }
    }

    public void FreeSpawnPoint(BoxOpener point)
    {
        if (occupiedPoints.ContainsKey(point.transform))
        {
            occupiedPoints[point.transform] = null;
            OnSpawnPointFreed?.Invoke();
        }
    }

    public bool IsThereFreeSpawnPoints()
    {
        bool hasFreePoints = false;
        foreach (var point in spawnPoints)
        {
            if (occupiedPoints[point.transform] == null) 
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
    void UnSpawnBox(BoxOpener box);
    void FreeSpawnPoint(BoxOpener point);

    bool IsThereFreeSpawnPoints();
}
