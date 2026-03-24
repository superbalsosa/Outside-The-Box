using System;
using UnityEngine;
using DependencyInjection;

public class PlayerInteractableController : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private LayerMask interactableLayer;
    private ICoreInteractable currentInteractable;
    private IErrorManager errorManager;

    #endregion

    #region UNITY_METHODS
    void Start()
    {
        errorManager = InterfaceDependencyInjector.Instance.Resolve<IErrorManager>();
        CleanData();
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
    private void OnTriggerExit(Collider other)
    {
        try
        {
            if (currentInteractable != null && 1 << other.gameObject.layer == interactableLayer)
            {
                if (other.gameObject.Equals(currentInteractable.GetGameObject()))
                {
                    currentInteractable.HideInteraction();
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