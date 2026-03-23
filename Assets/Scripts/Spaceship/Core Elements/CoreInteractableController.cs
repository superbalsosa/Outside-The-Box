using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Error;

namespace CoreInteractables
{
    public class CoreInteractableController : MonoBehaviour, ICoreInteractable
    {
        #region VARIABLES
        [SerializeField] private CoreInteractableSO interactableData;
        private List<Renderer> renderers = new List<Renderer>();
        #endregion

        #region UNITY_METHODS
        private void Start()
        {
            InitOutlines();
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitOutlines()
        {
            var childrenRenderers = GetComponentsInChildren<Renderer>();

            foreach (var childRender in childrenRenderers)
            {
                if (childRender.material.shader.name.Equals("StandardOutline_Built_In"))
                {
                    renderers.Add(childRender);
                }
            }
            ResetState();
        }
        private void ResetState() => HideInteraction();
        private void Interact()
        {
            //Interact code by interactableData.InteractableCoreType
        }
        private void ShowInteraction()
        {
            foreach (var render in renderers)
            {
                render.material.SetFloat("_OutlineWidth", interactableData.OutlineHover);
            }
        }
        private void HideInteraction()
        {
            foreach (var render in renderers)
            {
                render.material.SetFloat("_OutlineWidth", interactableData.OutlineBase);
            }
        }
        #endregion

        #region INTERFACE_METHODS
        ErrorUtility ICoreInteractable.Interact()
        {
            try
            {
                Interact();
                return ErrorUtility.Success(ErrorType.CoreInteractableInteract);
            }
            catch (Exception ex) { return ErrorUtility.Error(ErrorType.CoreInteractableInteract, ex.Message, ex.StackTrace); }
        }
        ErrorUtility ICoreInteractable.ShowInteraction()
        {
            try
            {
                ShowInteraction();
                return ErrorUtility.Success(ErrorType.CoreInteractableShowInteraction);
            }
            catch (Exception ex) { return ErrorUtility.Error(ErrorType.CoreInteractableShowInteraction, ex.Message, ex.StackTrace); }
        }
        ErrorUtility ICoreInteractable.HideInteraction()
        {
            try
            {
                HideInteraction();
                return ErrorUtility.Success(ErrorType.CoreInteractableHideInteraction);
            }
            catch (Exception ex) { return ErrorUtility.Error(ErrorType.CoreInteractableHideInteraction, ex.Message, ex.StackTrace); }
        }
        #endregion
    }
}
