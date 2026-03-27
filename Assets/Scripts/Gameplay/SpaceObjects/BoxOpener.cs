using DependencyInjection;
using UnityEditor;
using UnityEngine;

public class BoxOpener : MonoBehaviour, IListener
{
    private IBoxOpenerSpawner spawner;
    private Transform mySpawnPoint;

    [Header("Events")]
    [SerializeField] private EventType eventType;
    private IEventSystem eventManager;

    private void Start()
    {
        InitEvents();
        spawner = InterfaceDependencyInjector.Instance.Resolve<IBoxOpenerSpawner>();
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
    }

    private void InitEvents()
    {
        eventManager.SuscribeToEvent(eventType, this);
    }

    private void UnsuscribeToEvents()
    {
        eventManager.UnSuscribeToEvent(eventType, this);
    }

    public void Init(IBoxOpenerSpawner spawner, Transform point)
    {
        this.spawner = spawner;
        this.mySpawnPoint = point;
    }

    public void ExecuteListenerAction(bool isOn)
    {
        if (eventManager.GetCurrentEvent().Equals(eventType))
        {
            GiveReward();
            spawner.FreeSpawnPoint(mySpawnPoint);
            eventManager.ClearEvent();
            Destroy(gameObject);
        }
    }

    private void GiveReward()
    {
        int reward = Random.Range(0, 3);

        switch (reward)
        {
            case 0:
                Debug.Log(1);
                break;
            case 1:
                Debug.Log(2);
                break;
            case 2:
                Debug.Log(3);
                break;
        }
    }
}
