using UnityEngine;
using UnityEngine.Events;

public class BasicEventObject : MonoBehaviour
{

    [SerializeField] private float resolutionTime = 3f;
    [SerializeField] private UnityEvent onResolved;

    private bool playerInside;

    public bool isEventResolved = false;
    

    IDragAndDrop dragAndDrop;

    protected virtual void Awake()
    {
    }

    private void Start()
    {
        dragAndDrop = DependencyInjection.InterfaceDependencyInjector.Instance.Resolve<IDragAndDrop>();
        
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (playerInside && !dragAndDrop.IsDragging)
        {
            resolutionTime -= Time.deltaTime;
            if (resolutionTime <= 0)
            {
                onResolved.Invoke();
            }
        }
    }

    public void ResolveBasicObject()
    {
        isEventResolved = true;
        BasicEvent.Instance.RemoveObjectFromEvent();
        BasicEvent.Instance.OnEventCompleted.Invoke();
        ResolveEvent();
    }
    private void ResolveEvent()
    {
        Destroy(gameObject);
    }
}
