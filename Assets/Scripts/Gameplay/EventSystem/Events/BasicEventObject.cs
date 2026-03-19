using UnityEngine;
using UnityEngine.Events;

public class BasicEventObject : MonoBehaviour
{

    private bool playerInside;

    [SerializeField] private float resolutionTime = 3f;

    public bool isEventResolved = false;

    //IBasicEvent basicEvent;

    //private void Awake()
    //{
    //    basicEvent = DependencyInjection.InterfaceDependencyInjector.Instance.Resolve<IBasicEvent>();
    //}


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (playerInside)
        {
            resolutionTime -= Time.deltaTime;
            if (resolutionTime <= 0)
            {
                isEventResolved = true;
                BasicEvent.Instance.RemoveObjectFromEvent();
                BasicEvent.Instance.OnEventCompleted.Invoke();
                ResolveEvent();
            }
        }
    }

    private void ResolveEvent()
    {
        Destroy(gameObject);
    }
}
