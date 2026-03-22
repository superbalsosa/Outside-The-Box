using System;
using UnityEngine;
using Utilities.Error;

namespace CoreInteractables
{
    public class CoreInteractableController : MonoBehaviour, ICoreInteractable
    {
        [SerializeField] private CoreInteractableSO interactableData;
        private Material material;

        private void Start()
        {
            
        }
        //Start should be here but is not showing it
        #region INTERFACE_METHODS
        ErrorUtility ICoreInteractable.Interact()
        {
            try
            {
                return ErrorUtility.Success(ErrorType.CoreInteractableInteract);
            }
            catch (Exception ex)
            {
                return ErrorUtility.Error(ErrorType.CoreInteractableInteract, ex.Message);
            }
        }
        ErrorUtility ICoreInteractable.ShowInteraction()
        {
            try
            {
                return ErrorUtility.Success(ErrorType.CoreInteractableInteract);
            }
            catch (Exception ex)
            {
                return ErrorUtility.Error(ErrorType.CoreInteractableInteract, ex.Message);
            }
        }
        ErrorUtility ICoreInteractable.HideInteraction()
        {
            try
            {
                return ErrorUtility.Success(ErrorType.CoreInteractableInteract);
            }
            catch (Exception ex)
            {
                return ErrorUtility.Error(ErrorType.CoreInteractableInteract, ex.Message);
            }
        }
        #endregion
    }
}
