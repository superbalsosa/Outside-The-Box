using DependencyInjection;
using System;
using UnityEngine;

public class DebugGameController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool isDebugModeOn = false;
    [SerializeField] private float accelerationAmount = 10f;
    private IShipManager shipManager;
    void Start()
    {
        shipManager = InterfaceDependencyInjector.Instance.Resolve<IShipManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDebugModeOn) return;

        AccelerateDebug();
    }

    private void AccelerateDebug()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            shipManager.Accelerate(accelerationAmount, true);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            shipManager.Accelerate(accelerationAmount, false);
        }
    }
#endif
}
