using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    [Header("Impulse configuration")]
    public float forceAmount = 2f;
    public float torqueAmount = 0.5f;
    public float returnForce = 0.8f;

    [Header("Limits")]
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
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.1f;
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
            Vector3 orbitDirection = Vector3.Cross(directionToAnchor, transform.up);
            rb.AddForce((directionToAnchor + orbitDirection * 0.5f) * returnForce);
        }
    }
    /// <summary>
    /// It prevents the object from coming to a complete stop.
    /// </summary>
    private void MaintainInertia()
    {
        if (rb.linearVelocity.magnitude < 0.2f)
        {
            rb.AddForce(Random.insideUnitSphere * 0.1f, ForceMode.Acceleration);
        }
    }
}