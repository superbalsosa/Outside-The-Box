using DependencyInjection;
using TMPro;
using UnityEngine;

public class UITextController : MonoBehaviour, IListener
{
    [SerializeField] private EventType eventToRegistry;
    [SerializeField] private string preText;
    [SerializeField] private string postText;
    private IEventSystem eventSystem;
    private IShipManager shipManager;
    private TextMeshProUGUI textMeshPro;
    private bool isUpdating = true;


    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        eventSystem = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        eventSystem.SuscribeToEvent(eventToRegistry, this);
        shipManager = InterfaceDependencyInjector.Instance.Resolve<IShipManager>();
    }
    void Update()
    {
        if (!isUpdating) return;

        textMeshPro.text = preText + Mathf.RoundToInt(shipManager.GetCurrentMetersLeft()).ToString() + postText;
    }
    private void OnDisable()
    {
        eventSystem.UnSuscribeToEvent(eventToRegistry, this);
    }
    void IListener.ExecuteListenerAction(bool isOn)
    {
        if (eventSystem.GetCurrentEvent() == eventToRegistry && !isOn)
        {
            isUpdating = false;
        }
    }
}
