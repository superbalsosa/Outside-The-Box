using DependencyInjection;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpaceObject : MonoBehaviour, IInteract
{
    [Header("Impulse configuration")]
    public float forceAmount = 2.25f;
    public float torqueAmount = 0.5f;
    public float returnForce = 0.8f;
    public Vector3 forceDirection;

    [Header("Limits")]
    public float minSpeed = 0.25f;
    public float maxSpeed = 1f;
    public float maxDistance = 5f;

    [SerializeField] private Vector3 startScale;
    private Vector3 startPosition;
    public Rigidbody rb;

    [Header("Interact Settings")]
    [SerializeField] private DoorControler _doorControler;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private bool isLeftSide;
    [SerializeField] private int Value = 1;

    IBoxOpenerSpawner boxOpenerSpawner;
    private void OnEnable()
    {
        transform.localScale = startScale;
    }
    protected virtual void Start()
    {
        InitializePhysics();
        ApplyInitialImpulse();
        SetDoorReference();
        boxOpenerSpawner = InterfaceDependencyInjector.Instance.Resolve<IBoxOpenerSpawner>();
    }
    protected virtual void FixedUpdate()
    {
        HandleReturnToAnchor();
        //LimitVelocity();
        MaintainInertia();
    }

    private void SetDoorReference()
    {
        if (isLeftSide)
        {
            _doorControler = GameObject.FindGameObjectWithTag("LeftDoor").GetComponent<DoorControler>();
        }
        else
        {
            _doorControler = GameObject.FindGameObjectWithTag("RightDoor").GetComponent<DoorControler>();
        }
    }
    /// <summary>
    /// Configure the inital Rigidbody parameters.
    /// </summary>
    private void ApplyInitialImpulse()
    {
        Vector3 randomForce = new Vector3(Random.Range(0, forceDirection.x), Random.Range(0, forceDirection.y), Random.Range(3, forceDirection.z));
        rb.AddForce(randomForce * forceAmount, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * torqueAmount, ForceMode.Impulse);
    }
    /// <summary>
    /// Apply random initial thrust and rotation.
    /// </summary>
    private void InitializePhysics()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        rb.useGravity = false;
        rb.linearDamping = 0.01f;
        rb.angularDamping = 0.05f;
    }
    /// <summary>
    /// Calculate and apply forces so tha the object orbits close to its initial point.
    /// </summary>
    private void HandleReturnToAnchor()
    {
        float distance = Vector3.Distance(transform.position, startPosition);

        if (distance > maxDistance)
        {
            Vector3 directionToAnchor = (startPosition - transform.position).normalized;

            rb.AddForce(directionToAnchor * returnForce, ForceMode.Acceleration);
        }
    }
    /// <summary>
    /// If the object moves too slowly, receive a tiny impulse to keep the vacuum illusion.
    /// </summary>
    private void MaintainInertia()
    {
        if (rb.linearVelocity.magnitude < minSpeed)
        {
            rb.AddForce(Random.insideUnitSphere * 0.1f, ForceMode.Acceleration);
        }
    }
    /// <summary>
    /// Clamps the velocity to ensure the object never moves faster tha the maxSpeed.
    /// </summary>
    private void LimitVelocity()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    public void Interact()
    {
        onInteract.Invoke();
    }

    public void BoxInteract()
    {
        float duration = Random.Range(1f, 2f);
        StartCoroutine(CollectObject(duration));

    }

    private IEnumerator CollectObject(float duration)
    {
        if (_doorControler.wasDoorOpen && boxOpenerSpawner.IsThereFreeSpawnPoints())
        {
            float time = 0;

            while (time < duration)
            {
                transform.position = Vector3.Lerp(transform.position, _doorControler.transform.position, time / duration);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            ObjectPoolManager.ReturnToPool(this.gameObject);
            boxOpenerSpawner.SpawnBox();
        }
    }

    public DoorControler GetDoorControler()
    {
        return _doorControler;
    }
}