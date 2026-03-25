using DependencyInjection;
using UnityEngine;
using UnityEngine.Device;

public class DoorControler : MonoBehaviour, IListener
{
    [Header("Door Settings")]
    [SerializeField] private bool isLeftDoor;
    [SerializeField] private Transform tpPositionOutside;
    [SerializeField] private Transform tpPositionInside;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private bool wasDoorOpen;

    [Header("Events")]
    [SerializeField] private EventType eventToOpenClose;
    private IEventSystem eventManager;
    private IDragAndDrop dragAndDrop;

    void Start()
    {
        InitEvents();
        doorAnimator = GetComponent<Animator>();
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (isLeftDoor && !wasDoorOpen)
            {
                doorAnimator.SetTrigger("OpenDoor");
                player.transform.position = tpPositionOutside.position;
                dragAndDrop.ChangeBoundriesLeft();
                wasDoorOpen = isOn;
            }
            else if (isLeftDoor && wasDoorOpen)
            {
                doorAnimator.SetTrigger("CloseDoor");
                player.transform.position = tpPositionInside.position;
                dragAndDrop.ResetBoundries();
                wasDoorOpen = isOn;
            }
            if (!isLeftDoor)
            {
                doorAnimator.SetTrigger("OpenDoor");
                player.transform.position = tpPositionOutside.position;
                dragAndDrop.ChangeBoundriesRight();
                wasDoorOpen = isOn;
            }
            else if (!isLeftDoor && wasDoorOpen)
            {
                doorAnimator.SetTrigger("OpenDoor");
                player.transform.position = tpPositionOutside.position;
                dragAndDrop.ResetBoundries();
                wasDoorOpen = isOn;
            }

        }
    }
}
