using DependencyInjection;
using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using UnityEngine;

public class CameraRaycastController : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float raycastDistance = 250f;
    private IEventSystem eventManager;

    private void Start()
    {
        SetupReferences();
    }

    private void SetupReferences()
    {
        mainCamera = Camera.main;
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && eventManager.GetCurrentEvent() == EventType.None)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, interactableLayer))
        {
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2f);
            DrawDebugSphere(hit.point, 0.3f, Color.green);
#endif         
            if (hit.collider.TryGetComponent<IInteract>(out var interactable))
            {
                interactable.Interact(); 
            }

            if (hit.collider.TryGetComponent<IDragAndDrop>(out var draggable))
            {
                draggable.StartDrag();
                return;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.blue, 2f);
            Vector3 endPoint = ray.origin + ray.direction * raycastDistance;
            DrawDebugSphere(endPoint, 0.3f, Color.red);
#endif
        }
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
