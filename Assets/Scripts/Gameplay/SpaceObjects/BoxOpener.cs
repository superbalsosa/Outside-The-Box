using DependencyInjection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxOpener : MonoBehaviour, IListener
{
    private IBoxOpenerSpawner spawner;

    [Header("Events")]
    [SerializeField] private EventType openBoxEvent;
    private IEventSystem eventManager;

    public GameObject BoxGameObject;
    private void Start()
    {
        InitEvents();
        spawner = InterfaceDependencyInjector.Instance.Resolve<IBoxOpenerSpawner>();
    }

    //private void OnEnable()
    //{
    //    InitEvents();
    //}

    private void OnDisable()
    {
        UnsuscribeToEvents();
    }

    private void InitEvents()
    {
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        eventManager.SuscribeToEvent(openBoxEvent, this);
    }

    private void UnsuscribeToEvents()
    {
        eventManager.UnSuscribeToEvent(openBoxEvent, this);
    }

    public void Init(IBoxOpenerSpawner spawner)
    {
        this.spawner = spawner;   
    }

    public void ExecuteListenerAction(bool isOn)
    {
        if (isOn && eventManager.GetCurrentEvent().Equals(openBoxEvent))
        {
            GiveReward();
            DisableAllChildren(this.transform);
            eventManager.SetEvent(openBoxEvent, false);
            eventManager.ClearEvent();
            spawner.FreeSpawnPoint(this);
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    void DisableAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
            DisableAllChildren(child);
        }
    }

    private void GiveReward()
    {
        int reward = Random.Range(0, 3);

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

}
