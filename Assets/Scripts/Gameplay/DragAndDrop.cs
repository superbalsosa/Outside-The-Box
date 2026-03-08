using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [Header("Movement settings")]
    public float speed = 10f;

    private Rigidbody rb;
    private Camera mainCamera;
    private float zDistance;
    private Vector3 offset;

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
    /// Calculates the Z-depth relative to the camera and the offset 
    /// between the click point and the object center.
    /// </summary>
    private void PrepareDrag()
    {
        zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    /// <summary>
    /// Calculates the target position by combining the mouse world position 
    /// and the then moves the rigidbody using MovePosition to collision detection.
    /// </summary>
    private void PerformDrag()
    {
        Vector3 targetPos = GetMouseWorldPos() + offset;

        rb.MovePosition(targetPos);
    }
    /// <summary>
    /// Converts 2D mouse screen coordinates into a 3D world point using the
    /// Z-depth calculates at the start of the drag.
    /// </summary>
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zDistance;

        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}