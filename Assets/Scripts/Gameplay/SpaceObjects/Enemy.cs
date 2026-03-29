using System.Collections;
using UnityEngine;
using static ObjectPoolManager;

public class Enemy : SpaceObject
{
    [Header("Enemy settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spaceShip;
    [SerializeField] private int maxBulletsCount = 6;
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float enemyLifetime = 90f;

    private int bulletCount;
    private float shootingTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        bulletCount = Random.Range(1, maxBulletsCount);
    }

    private void OnEnable()
    {
        spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").transform;
    }

    // Update is called once per frame
    void Update()
    { 
        if (bulletCount > 0)
        {
            shootingTimer += Time.deltaTime;

            if (shootingTimer >= shootingInterval)
            {
                ShootPlayer();
                shootingTimer = 0f;
            }
        }
        else
        {
            StartCoroutine(Retretenemy(enemyLifetime));
        }
    }

    void ShootPlayer()
    {
        Vector3 direction = (spaceShip.position - transform.position).normalized;
        GameObject bullet = ObjectPoolManager.SpawnSpaceObject(bulletPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.EnemyBullets);
        SetBulletDamage(bullet.GetComponent<EnemyBullet>());
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.linearVelocity = direction * bulletSpeed;
        bulletCount--;

    }

    public void DefetEnemy()
    {
        if (SpaceShipManager.Instance.BattleModeActive)
        {
            StartCoroutine(Retretenemy(enemyLifetime));
        }
    }

    private void SetBulletDamage(EnemyBullet enemyBullet)
    {
        if (base.GetDoorControler().wasDoorOpen)
        {
            enemyBullet.SetDamage(20);
        }
        else
        {
            enemyBullet.SetDamage(10);
        }
    }

    private IEnumerator Retretenemy(float duration)
    {

        float time = 0;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time / duration);
            ObjectPoolManager.ReturnToPool(this.gameObject);
            yield return null;
        }
    }

}
