using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObjectSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Vector3 areaCenter;
    [SerializeField] private Vector3 areaSize;
    [SerializeField] private Vector3 enemyAreaCenter;
    [SerializeField] private Vector3 enemyAreaSize;

    #region Object Spawn Settings
    [Header("Object Spawn Settings")]
    [SerializeField] private List<GameObject> spaceObjectsToSpawn = new List<GameObject>();
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float maxSpawnInterval = 10f;
    [SerializeField] private float disableTime = 10f;
    [SerializeField] private int randomSpawnQuantity = 4;
    [SerializeField] private int maxObjects = 10;
    [SerializeField] ObjectPoolManager.PoolType poolType;
    #endregion

    #region Enemy Spawn Settings
    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<GameObject> enemysToSpawn = new List<GameObject>();
    [SerializeField] private float enemySpawnInterval = 2f;
    [SerializeField] private float enemyMaxSpawnInterval = 10f;
    [SerializeField] private int enemyRandomSpawnQuantity = 3;
    [SerializeField] private int enemyMaxObjects = 10;
    [SerializeField] ObjectPoolManager.PoolType enemyPoolType;
    #endregion

    private Quaternion spawnRotation = Quaternion.identity;
    private float timer;
    private float enemyTimer;

    Queue<GameObject> recentSpaceObjects = new Queue<GameObject>();
    Queue<GameObject> recentEnemys = new Queue<GameObject>();

  
    private void Update()
    {
        timer += Time.deltaTime;
        enemyTimer += Time.deltaTime;
        if (timer >= spawnInterval)
        {            
            timer = 0f;
            spawnInterval = Random.Range(maxSpawnInterval/2, maxSpawnInterval);
            SpawnSpaceObject();
        }

        if (enemyTimer >= enemySpawnInterval && SpaceShipManager.Instance.EnemyCount < SpaceShipManager.Instance.MaxEnemyCount)
        {
            enemyTimer = 0f;
            enemySpawnInterval = Random.Range(enemyMaxSpawnInterval / 2, enemyMaxSpawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnSpaceObject()
    {
        if (spaceObjectsToSpawn.Count == 0) return;

        int spawnAmount = Random.Range(0, randomSpawnQuantity);

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
            Quaternion spawnRot = Quaternion.Euler(0, 85, 0);
            GameObject spawnedObject = ObjectPoolManager.SpawnSpaceObject(objectToSpawn, spawnPos, spawnRot, poolType);

            recentSpaceObjects.Enqueue(spawnedObject);
            StartCoroutine(DisableAfterTime(spawnedObject));

            if (recentSpaceObjects.Count > maxObjects)
            {
                recentSpaceObjects.Dequeue();
            }
        }       
    }

    private void SpawnEnemy()
    {

        if (enemysToSpawn.Count == 0) return;

        int spawnAmount = Random.Range(0, enemyRandomSpawnQuantity);

        for (int i = 0; i < spawnAmount; i++)
        {
        
            List<GameObject> availableObjects = new List<GameObject>(enemysToSpawn);
            foreach (var recent in recentEnemys)
            {
                if (availableObjects.Contains(recent))
                {
                    availableObjects.Remove(recent);
                }
            }

            if (availableObjects.Count == 0)
            {
                if (recentEnemys.Count > 0) recentEnemys.Dequeue();

                availableObjects = new List<GameObject>(enemysToSpawn);
                foreach (var recent in recentEnemys)
                {
                    if (availableObjects.Contains(recent))
                    {
                        availableObjects.Remove(recent);
                    }
                }
            }

            GameObject objectToSpawn = enemysToSpawn[Random.Range(0, enemysToSpawn.Count)];

            Vector3 spawnPos = EnemySpawnPosition();
            GameObject spawnedObject = ObjectPoolManager.SpawnSpaceObject(objectToSpawn, spawnPos, spawnRotation, enemyPoolType);

            SpaceShipManager.Instance.EnemyCount++;
            

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

    public Vector3 EnemySpawnPosition()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-enemyAreaSize.x / 2, enemyAreaSize.x / 2),
            Random.Range(-enemyAreaSize.y / 2, enemyAreaSize.y / 2),
            Random.Range(-enemyAreaSize.z / 2, enemyAreaSize.z / 2)
        );
        return randomPos += enemyAreaCenter;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter, areaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemyAreaCenter, enemyAreaSize);
    }
#endif
}
