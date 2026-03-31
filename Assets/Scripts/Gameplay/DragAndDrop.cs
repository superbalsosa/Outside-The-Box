using Audio.Interfaces;
using DependencyInjection;
using UnityEngine;

public class DragAndDrop : MonoBehaviour, IDragAndDrop
{
    [Header("Boundary Settings")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool lookAtMovement = true;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 offset;
    private Vector4 savedBoundries;
    private float lockedY;

    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private float raycastDistance = 150f;
    private bool isDragging = false;
    private IEventSystem eventManager;

    [SerializeField] private Animator animator;
    [SerializeField] private string isDraggingParameter = "IsDragging";
    public bool IsDragging { get => isDragging; }

    private void Awake()
    {
        InterfaceDependencyInjector.Instance.Register<IDragAndDrop>(() => this);
    }

    private void Start()
    {
        SetupReferences();
        savedBoundries = SaveBoundries();
    }
    //private void OnMouseDown()
    //{
    //    PrepareDrag();
    //}
    //private void OnMouseDrag()
    //{
    //    PerformDrag();
    //}

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SetDraggingAnimation(false);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && isDragging)
        {
            PerformDrag();
        }
    }

    public void StartDrag()
    {
        if (eventManager.GetCurrentEvent() != EventType.None) return;

        isDragging = true;
        PrepareDrag();
        SetDraggingAnimation(true);
    }

    private void SetDraggingAnimation(bool value)
    {
        if (animator == null) return;
        animator.SetBool(isDraggingParameter, value);
    }

    private void SetupReferences()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
    }
    /// <summary>
    /// Initialize the drag by capturing the locked Y height and
    /// calculating the offset using a raycast intersection.
    /// </summary>
    private void PrepareDrag()
    {
        offset = transform.position - GetMouseWorldPos();

        lockedY = transform.position.y;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    /// <summary>
    /// Updates the object position based on the mouse intersection with
    /// the horizontal plane.
    /// </summary>
    private void PerformDrag()
    {
        Vector3 targetPos = GetMouseWorldPos() + offset;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);
        targetPos.y = lockedY;

        if (lookAtMovement)
        {
            Vector3 direction = (targetPos - transform.position);
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));
            }
        }
        rb.MovePosition(targetPos);
    }
    /// <summary>
    /// Projects a ray from the camera through the mouse position to find
    /// the intersection point with a visual horizontal plane at the 
    /// locked Y height.
    /// </summary> 
    /// <returns> the 3D position where the mouse points at the specific Y height.</returns>
    private Vector3 GetMouseWorldPos()
    {
        Plane horizontalPlane = new Plane(Vector3.up, new Vector3(0, lockedY, 0));

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (horizontalPlane.Raycast(ray, out float entry))
        {
            return ray.GetPoint(entry);
        }

        return transform.position;
    }

//    private void TryStartDrag()
//    {
//        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

//        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, draggableLayer))
//        {
//#if UNITY_EDITOR
//            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2f);
//            DrawDebugSphere(hit.point, 0.3f, Color.green);
//#endif
//            if (hit.collider.gameObject == gameObject)
//            {
//                isDragging = true;
//                PrepareDrag();
//            }
//        }
//        else
//        {
//            Debug.Log("No hit detected on draggable layer.");
//#if UNITY_EDITOR
//            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.blue, 2f);
//            Vector3 endPoint = ray.origin + ray.direction * raycastDistance;
//            DrawDebugSphere(endPoint, 0.3f, Color.red);
//#endif
//        }
//    }
    /// <summary>
    /// Verifies if a given position is within the defined X and Z limits.
    /// </summary>
    /// <param name="pos">The position to check.</param>
    /// <returns>True if the position is within the square boundaries.</returns>
    private bool IsInsideBoundaries(Vector3 pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ;
    }
    private Vector4 SaveBoundries()
    {
        return new Vector4(minX, maxX, minZ, maxZ);
    }
    public void ChangeBoundriesLeft()
    {
        minX = 26f;
        maxX = 50f;
        minZ = -34f;
        maxZ = 12f;
    }

    public void ChangeBoundriesRight()
    {
        minX = -50f;
        maxX = -26f;
        minZ = -34f;
        maxZ = 12f;
    }

    public void ResetBoundries()
    {
        minX = savedBoundries.x;
        maxX = savedBoundries.y;
        minZ = savedBoundries.z;
        maxZ = savedBoundries.w;
    }

    #region Debug
    void DrawDebugSphere(Vector3 position, float radius, Color color)
    {
        float step = 10f;

        for (int i = 0; i < 360; i += (int)step)
        {
            float rad = Mathf.Deg2Rad * i;
            float nextRad = Mathf.Deg2Rad * (i + step);

            Debug.DrawLine(
                position + new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius),
                position + new Vector3(Mathf.Cos(nextRad) * radius, 0, Mathf.Sin(nextRad) * radius),
                color, 2f
            );

            Debug.DrawLine(
                position + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0),
                position + new Vector3(Mathf.Cos(nextRad) * radius, Mathf.Sin(nextRad) * radius, 0),
                color, 2f
            );

            Debug.DrawLine(
                position + new Vector3(0, Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius),
                position + new Vector3(0, Mathf.Cos(nextRad) * radius, Mathf.Sin(nextRad) * radius),
                color, 2f
            );
        }
    }
    #endregion
}

public interface IDragAndDrop
{
    bool IsDragging { get; }
    void StartDrag();
    void ChangeBoundriesLeft();
    void ChangeBoundriesRight();
    void ResetBoundries();
}