using DependencyInjection;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;

public class PlayerInteractableController : MonoBehaviour, IListener
{
    #region VARIABLES

    [SerializeField] private LayerMask interactableLayer;
    private ICoreInteractable currentInteractable;
    //private IErrorManager errorManager;
    private IDragAndDrop dragAndDrop;
    private IEventSystem eventManager;
    //private bool hasInteracted = false;
    #endregion

    #region UNITY_METHODS
    void Start()
    {
        //errorManager = InterfaceDependencyInjector.Instance.Resolve<IErrorManager>();
        dragAndDrop = InterfaceDependencyInjector.Instance.Resolve<IDragAndDrop>();
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        InitEvents();
        CleanData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && eventManager.GetCurrentEvent().Equals(EventType.InteractableControlTable)) {
            eventManager.SetEvent(EventType.InteractableControlTable, false);
            eventManager.ClearEvent();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (currentInteractable != null && !currentInteractable.GetGameObject().activeInHierarchy)
            {
                currentInteractable = null;
            }
            if ((interactableLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                if (other.TryGetComponent<ICoreInteractable>(out var interactable))
                {
                    if (currentInteractable != null && currentInteractable != interactable)
                        currentInteractable.HideInteraction();

                    currentInteractable = interactable;
                    currentInteractable.ShowInteraction();
                }
            }
        }
        catch (Exception ex) 
        {
            Debug.LogException(ex);
            //errorManager.WriteError(ErrorType.PlayerInteractableController_TriggerEnter, ex.Message, ex.StackTrace);
            throw ex;
        }
    }
    /* Deprecated: Used to trigger if the player has been dragged into the corresponding trigger
    private void OnTriggerStay(Collider other)
    {
        try
        {
            if (currentInteractable != null && (1 << other.gameObject.layer) == interactableLayer)
            {
                if (other.gameObject.Equals(currentInteractable.GetGameObject()))
                {
                    if (!hasInteracted && dragAndDrop.IsDragging == false)
                    {
                        currentInteractable.Interact();
                        hasInteracted = true; 
                    }
                }
            }
        }
        catch (Exception ex)
        {
            errorManager.WriteError(ErrorType.PlayerInteractableController_TriggerStay, ex.Message, ex.StackTrace);
            throw;
        }
    }
    */
    private void OnTriggerExit(Collider other)
    {
        try
        {
            if (!currentInteractable.IsUnityNull() && 1 << other.gameObject.layer == interactableLayer)
            {
                if (other.gameObject.Equals(currentInteractable.GetGameObject()))
                {
                    currentInteractable.HideInteraction();
                    //hasInteracted = false;
                    CleanData();
                }
            }
        }
        catch (Exception ex)
        {
            //errorManager.WriteError(ErrorType.PlayerInteractableController_TriggerExit, ex.Message, ex.StackTrace);
            throw ex;
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private void InitEvents()
    {
    //    eventManager.SuscribeToEvent(EventType.OpenBox, this);
    }
    private void CleanData()
    {
        currentInteractable = null;
    }

    void IListener.ExecuteListenerAction(bool isOn)
    {
        //if (eventManager.GetCurrentEvent().Equals(EventType.OpenBox) && !isOn && !currentInteractable.IsUnityNull())
        //{
        //    CleanData();
        //}
    }
    #endregion
}