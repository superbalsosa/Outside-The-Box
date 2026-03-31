using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
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

    [Header("Knockback settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDistance = 8f;

    [Header("Sound settings")]
    [SerializeField] private SoundData _attackSound;
    [SerializeField] private SoundData _deathSound;

    [Header("Damage Color settings")]
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitDuration = 0.2f;

    ISoundManager soundManager;

    private float shootingTimer;
    private bool isKnockback = false;
    private bool isDead = false;

    private Vector3 localScaleSave;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();

    }

    private void OnEnable()
    {
        enemyLife = 100;
        spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        HandleMovement();

        float distance = Vector3.Distance(transform.position, spaceShip.position);

        if (distance <= stopDistance)
        {
            shootingTimer += Time.deltaTime;

            if (shootingTimer >= shootingInterval)
            {
                ShootPlayer();
                shootingTimer = 0f;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    void ShootPlayer()
    {
        Vector3 direction = (spaceShip.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject bullet = ObjectPoolManager.SpawnSpaceObject(bulletPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.EnemyBullets);
        SetBulletDamage(bullet.GetComponent<EnemyBullet>());
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.linearVelocity = direction * bulletSpeed;

    }

    public void DefetEnemy()
    {
        if (enemyLife <= 0 && !isDead)
        {
            isDead = true;

            enemyLife = 0;

            int randomAmount = Random.Range(10, 35);
            soundManager.CreateSound().WithSoundData(_deathSound).Play();
            SpaceShipManager.Instance.ChangeStardust(randomAmount);
            SpaceShipManager.Instance.EnemyCount--;

            StartCoroutine(Retretenemy(3f));
        }
    }

    public void TakeDamage(int damage)
    {
        soundManager.CreateSound().WithSoundData(_attackSound).Play();
        enemyLife -= damage;
        ApplyKnockback();
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

    private void HandleMovement()
    {
        if (enemyLife <= 0) return;

        if (isKnockback) return;

        float distance = Vector3.Distance(transform.position, spaceShip.position);

        if (distance > stopDistance)
        {
            Vector3 direction = (spaceShip.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void ApplyKnockback()
    {
        Vector3 direction = (transform.position - spaceShip.position).normalized;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    private IEnumerator Retretenemy(float duration)
    {

        float time = 0;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        ObjectPoolManager.ReturnToPool(this.gameObject);
       
    }

}
