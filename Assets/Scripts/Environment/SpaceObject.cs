using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    [Header("Impulse configuration")]
    public float forceAmount = 2.25f;
    public float torqueAmount = 0.5f;
    public float returnForce = 0.8f;

    [Header("Limits")]
    public float minSpeed = 0.25f;
    public float maxSpeed = 1f;
    public float maxDistance = 5f;

    private Vector3 startPosition;
    private Rigidbody rb;
    
    public void Start()
    {
        InitializePhysics();
        ApplyInitialImpulse();
    }
    private void FixedUpdate()
    {
        HandleReturnToAnchor();
        LimitVelocity();
        MaintainInertia();
    }
    /// <summary>
    /// Configure the inital Rigidbody parameters.
    /// </summary>
    private void ApplyInitialImpulse()
    {
        rb.AddForce(Random.insideUnitSphere * forceAmount, ForceMode.Impulse);
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
}