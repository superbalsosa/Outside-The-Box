using UnityEngine;

public class DragAndDrop : MonoBehaviour
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

    private void Start()
    {
        SetupReferences();
    }
    private void OnMouseDown()
    {
        PrepareDrag();
    }
    private void OnMouseDrag()
    {
        PerformDrag();
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
    /// <summary>
    /// Verifies if a given position is within the defined X and Z limits.
    /// </summary>
    /// <param name="pos">The position to check.</param>
    /// <returns>True if the position is within the square boundaries.</returns>
    private bool IsInsideBoundaries(Vector3 pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2, transform.position.y, (minZ + maxZ) / 2);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}