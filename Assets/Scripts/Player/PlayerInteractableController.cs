using System;
using UnityEngine;
using Utilities.Error;

public class PlayerInteractableController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    private ICoreInteractable currentInteractable;

    void Start()
    {
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
            ErrorUtility.Error(ErrorType.PlayerInteractableControllerTriggerEnter, ex.Message, ex.StackTrace);
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
            ErrorUtility.Error(ErrorType.PlayerInteractableControllerTriggerExit, ex.Message, ex.StackTrace);
            throw ex;
        }
    }
    private void CleanData()
    {
        currentInteractable = null;
    }
}
