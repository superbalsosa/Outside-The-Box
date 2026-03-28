using DependencyInjection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
    }

    private void OnEnable()
    {
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        InitEvents();
    }

    private void OnDisable()
    {
        UnsuscribeToEvents();
    }

    private void InitEvents()
    {
        eventManager.SuscribeToEvent(eventType, this);
    }

    private void UnsuscribeToEvents()
    {
        spawner.OnSpawnPointFreed -= UpdateEvent;
        eventManager.UnSuscribeToEvent(eventType, this);
    }

    public void Init(IBoxOpenerSpawner spawner, Transform point)
    {
        this.spawner = spawner;
        this.mySpawnPoint = point;      
        spawner.OnSpawnPointFreed += UpdateEvent;
    }

    public void ExecuteListenerAction(bool isOn)
    {
        if (isOn && eventManager.GetCurrentEvent().Equals(eventType))
        {
            GiveReward();
            spawner.FreeSpawnPoint(mySpawnPoint);
            eventManager.SetEvent(eventType, false);
            eventManager.ClearEvent();
            Destroy(this.gameObject);
        }
    }

    private void GiveReward()
    {
        int reward = Random.Range(0, 2);

        switch (reward)
        {
            case 0:
                int randomAmount = Random.Range(10, 75);
                SpaceShipManager.Instance.ChangeStarDust(randomAmount);
                break;
            case 1:
                SpaceShipManager.Instance.GrabBatterys(1);
                break;
            case 2:
                Debug.Log("Box was empty");
                break;
        }
    }

    public void UpdateEvent()
    {
        InitEvents();
    }
}
