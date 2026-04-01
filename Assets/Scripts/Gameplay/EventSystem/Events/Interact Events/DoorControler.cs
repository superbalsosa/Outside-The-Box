using Audio;
using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
using System.Collections;
using UnityEngine;
using UnityEngine.Device;

public class DoorControler : MonoBehaviour, IListener
{
    [Header("Door Settings")]
    [SerializeField] private Vector3 OpenDoorPosition;
    [SerializeField] private Vector3 ClosedDoorPosition;
    [Header("Door Sounds")]
    [SerializeField] private SoundData openDoorSound;
    [SerializeField] private SoundData closeDoorSound;
    [SerializeField] private SoundData cantUseDoorSound;
    private bool isMooving;

    public bool wasDoorOpen;

    [Header("Events")]
    [SerializeField] private EventType eventToOpenClose;
    private IEventSystem eventManager;
    private IDragAndDrop dragAndDrop;
    private ISoundManager soundManager;

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
        soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        dragAndDrop = InterfaceDependencyInjector.Instance.Resolve<IDragAndDrop>();
        eventManager.SubscribeToEvent(eventToOpenClose, this);
    }
    private void UnsuscribeToEvents()
    {
        eventManager.UnSubscribeToEvent(eventToOpenClose, this);
    }

    public void ExecuteListenerAction(bool isOn)
    {
        if (eventManager.GetCurrentEvent().Equals(eventToOpenClose))
        {
            eventManager.ClearEvent();

            if (!wasDoorOpen && !isMooving)
            {
                soundManager.CreateSound().WithSoundData(openDoorSound).Play();
                isMooving = true;
                StartCoroutine(MoveDoor(OpenDoorPosition, 1f));
                wasDoorOpen = true;
            }
            else if (wasDoorOpen && !isMooving)
            {
                soundManager.CreateSound().WithSoundData(closeDoorSound).Play();
                isMooving = true;
                StartCoroutine(MoveDoor(ClosedDoorPosition, 1f));
                wasDoorOpen = false;
            }
            else
            {
                soundManager.CreateSound().WithSoundData(cantUseDoorSound).Play();
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
