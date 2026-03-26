using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class SpaceObjectSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Vector3 areaCenter;
    [SerializeField] private Vector3 areaSize;

    [SerializeField] private List<GameObject> spaceObjectsToSpawn = new List<GameObject>();
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float disableTime = 10f;
    [SerializeField] private int maxObjects = 10;

    private Quaternion spawnRotation = Quaternion.identity;
    private float timer;

    Queue<GameObject> recentSpaceObjects = new Queue<GameObject>();

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnSpaceObject();
        }
    }

    private void SpawnSpaceObject()
    {
        if (spaceObjectsToSpawn.Count == 0) return;

        int spawnAmount = Random.Range(1, 3);

        for (int i = 0; i < spawnAmount; i++)
        {
            //if (activeObjects.Count >= maxObjects)
            //    break;
            List<GameObject> availableObjects = new List<GameObject>(spaceObjectsToSpawn);
            foreach( var recent in recentSpaceObjects)
            {
                if (availableObjects.Contains(recent))
                {
                    availableObjects.Remove(recent);
                }
            }

            if (availableObjects.Count == 0)
            {
                if (recentSpaceObjects.Count > 0) recentSpaceObjects.Dequeue();

                availableObjects = new List<GameObject>(spaceObjectsToSpawn);
                foreach (var recent in recentSpaceObjects)
                {
                    if (availableObjects.Contains(recent))
                    {
                        availableObjects.Remove(recent);
                    }
                }
            }

            GameObject objectToSpawn = spaceObjectsToSpawn[Random.Range(0, spaceObjectsToSpawn.Count)];

            Vector3 spawnPos = SpawnPosition();
            GameObject spawnedObject = ObjectPoolManager.SpawnSpaceObject(objectToSpawn, spawnPos, spawnRotation, ObjectPoolManager.PoolType.None);

            recentSpaceObjects.Enqueue(spawnedObject);
            StartCoroutine(DisableAfterTime(spawnedObject));

            if (recentSpaceObjects.Count > maxObjects)
            {
                recentSpaceObjects.Dequeue();
            }
        }       
    }

    private IEnumerator DisableAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(disableTime);

        ObjectPoolManager.ReturnToPool(obj);
    }

    public Vector3 SpawnPosition()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );

        return randomPos += areaCenter;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
#endif
}
