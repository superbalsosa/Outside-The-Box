using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<ObjectPoolInfo> ObjectPools = new List<ObjectPoolInfo>();

    private GameObject objectPoolEmptyHolder;
    private static GameObject spaceObjectEmpty;
    private static GameObject enemyEmpty;
    private static GameObject boxOpenerEmpty;

    public enum PoolType
    {
        None,
        Boxes,
        Debris,
        Satellite,
        Enemys,
        BoxeOpener,
        EnemyBullets,
    }

    public static PoolType PoolingType;

    private void Awake()
    {
        SetUpEmpties();
    }

    private void SetUpEmpties()
    {
        objectPoolEmptyHolder = new GameObject("ObjectPoolEmptyHolder");

        spaceObjectEmpty = new GameObject("SpaceObjectEmpty");
        spaceObjectEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        enemyEmpty = new GameObject("EnemyEmpty");
        enemyEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        boxOpenerEmpty = new GameObject("BoxOpenerEmpty");
        boxOpenerEmpty.transform.SetParent(objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnSpaceObject(GameObject spaceObjectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        ObjectPoolInfo poolInfo = ObjectPools.Find(p => p.LookUpString == spaceObjectToSpawn.name);

        if (poolInfo == null)
        {
            poolInfo = new ObjectPoolInfo() { LookUpString = spaceObjectToSpawn.name };
            ObjectPools.Add(poolInfo);
        }

        GameObject SpawnableObjects = null;
        foreach (GameObject obj in poolInfo.InactiveObjects)
        {
            if (obj != null)
            {
                SpawnableObjects = obj;
                break;
            }
        }

        if (SpawnableObjects == null)
        {
            GameObject parentObject = SetParentObject(poolType);

            SpawnableObjects = Instantiate(spaceObjectToSpawn, spawnPosition, spawnRotation);

            if (parentObject != null)
            {
                SpawnableObjects.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            SpawnableObjects.transform.position = spawnPosition;
            SpawnableObjects.transform.rotation = spawnRotation;
            poolInfo.InactiveObjects.Remove(SpawnableObjects);
            SpawnableObjects.SetActive(true);
        }

        return SpawnableObjects;
    }

    public static void ReturnToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7); // esto es para sacar el (Clone) de los gameObject

        ObjectPoolInfo pool = ObjectPools.Find(p => p.LookUpString == goName);

        if (pool == null)
        {
            //Debug.LogWarning("Trying to realese an object that is not pooled " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.None:
                return null;

            case PoolType.Boxes:
                return spaceObjectEmpty;

            case PoolType.Debris:
                return spaceObjectEmpty;

            case PoolType.Satellite:
                return spaceObjectEmpty;

            case PoolType.Enemys:
                return enemyEmpty;

            case PoolType.BoxeOpener:
                return boxOpenerEmpty;

            default:
                return null;
        }
    }
}

public class ObjectPoolInfo
{
    public string LookUpString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
