using UnityEngine;

public class DragAndDrop : MonoBehaviour, IDragAndDrop
{
    [Header("Boundary Settings")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 offset;
    private float lockedY;

    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private float raycastDistance = 150f;
    private bool isDragging = false;

    public bool IsDragging { get => isDragging; }

    private void Start()
    {
        SetupReferences();
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
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && isDragging)
        {
            PerformDrag();
        }
    }
    private void SetupReferences()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
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

    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, draggableLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2f);
            if (hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                PrepareDrag();
            }
        }
    }
    /// <summary>
    /// Verifies if a given position is within the defined X and Z limits.
    /// </summary>
    /// <param name="pos">The position to check.</param>
    /// <returns>True if the position is within the square boundaries.</returns>
    private bool IsInsideBoundaries(Vector3 pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ;
    }
}

public interface IDragAndDrop
{
    bool IsDragging { get; }
}