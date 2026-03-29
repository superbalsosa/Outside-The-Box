using DependencyInjection;
using System.Collections;
using UnityEngine;
using UnityEngine.Device;

public class DoorControler : MonoBehaviour, IListener
{
    [Header("Door Settings")]
    [SerializeField] private Vector3 OpenDoorPosition;
    [SerializeField] private Vector3 ClosedDoorPosition;
    private bool isMooving;

    public bool wasDoorOpen;

    [Header("Events")]
    [SerializeField] private EventType eventToOpenClose;
    private IEventSystem eventManager;
    private IDragAndDrop dragAndDrop;

    void Start()
    {
        InitEvents();
        //ClosedDoorPosition = gameObject.transform.position;
    }

    private void OnDisable()
    {
        UnsuscribeToEvents();
    }

    private void InitEvents()
    {
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        dragAndDrop = InterfaceDependencyInjector.Instance.Resolve<IDragAndDrop>();
        eventManager.SuscribeToEvent(eventToOpenClose, this);
    }
    private void UnsuscribeToEvents()
    {
        eventManager.UnSuscribeToEvent(eventToOpenClose, this);
    }

    public void ExecuteListenerAction(bool isOn)
    {
        if (eventManager.GetCurrentEvent().Equals(eventToOpenClose))
        {
            if (!wasDoorOpen && !isMooving)
            {
                eventManager.ClearEvent();
                isMooving = true;
                StartCoroutine(MoveDoor(OpenDoorPosition, 1f));
                wasDoorOpen = true;
            }
            else if (wasDoorOpen && !isMooving)
            {
                eventManager.ClearEvent();
                isMooving = true;
                StartCoroutine(MoveDoor(ClosedDoorPosition, 1f));
                wasDoorOpen = false;
            }
            else
            {
                eventManager.ClearEvent();
            }
        }
    }

    private IEnumerator MoveDoor(Vector3 targetPosition, float duration)
    {
        Vector3 startPos = transform.position;
        float time = 0;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        
        isMooving = false;
    }
}
