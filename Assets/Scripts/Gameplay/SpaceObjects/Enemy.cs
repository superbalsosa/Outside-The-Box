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
    [SerializeField] private int enemyLife = 100;

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
        enemyLife = 100;
        spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").transform;
    }

    // Update is called once per frame
    void Update()
    { 

        shootingTimer += Time.deltaTime;

        if (shootingTimer >= shootingInterval)
        {
            ShootPlayer();
            shootingTimer = 0f;
        }

        DefetEnemy();

    }

    void ShootPlayer()
    {
        Vector3 direction = (spaceShip.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject bullet = ObjectPoolManager.SpawnSpaceObject(bulletPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.EnemyBullets);
        SetBulletDamage(bullet.GetComponent<EnemyBullet>());
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.linearVelocity = direction * bulletSpeed;
        bulletCount--;

    }

    public void DefetEnemy()
    {
        if (enemyLife <= 0)
        {
            enemyLife = 0;
            int randomAmount = Random.Range(10, 35);
            SpaceShipManager.Instance.ChangeStarDust(randomAmount);
            StartCoroutine(Retretenemy(3f));
        }
    }

    public void TakeDamage(int damage)
    {
        enemyLife -= damage;
        DefetEnemy();
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
