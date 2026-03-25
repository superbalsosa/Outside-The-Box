using System;
using UnityEngine;
using DependencyInjection;

public class PlayerInteractableController : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private LayerMask interactableLayer;
    private ICoreInteractable currentInteractable;
    private IErrorManager errorManager;
    private IDragAndDrop dragAndDrop;
    private IEventSystem eventManager;
    //private bool hasInteracted = false;
    #endregion

    #region UNITY_METHODS
    void Start()
    {
        errorManager = InterfaceDependencyInjector.Instance.Resolve<IErrorManager>();
        dragAndDrop = InterfaceDependencyInjector.Instance.Resolve<IDragAndDrop>();
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        CleanData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && eventManager.GetCurrentEvent().Equals(EventType.InteractableControlTable)) {
            eventManager.SetEvent(EventType.InteractableControlTable, false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (currentInteractable == null && 1 << other.gameObject.layer == interactableLayer)
            {
                if (other.TryGetComponent<ICoreInteractable>(out var interactable))
                {
                    currentInteractable = interactable;
                    interactable.ShowInteraction();
                }
            }
        }
        catch (Exception ex) 
        {
            errorManager.WriteError(ErrorType.PlayerInteractableController_TriggerEnter, ex.Message, ex.StackTrace);
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
            if (currentInteractable != null && 1 << other.gameObject.layer == interactableLayer)
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
            errorManager.WriteError(ErrorType.PlayerInteractableController_TriggerExit, ex.Message, ex.StackTrace);
            throw ex;
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private void CleanData()
    {
        currentInteractable = null;
    }
    #endregion
}