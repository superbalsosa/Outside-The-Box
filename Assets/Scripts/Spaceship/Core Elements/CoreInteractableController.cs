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
        private ErrorUtility errorUtility;
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
        void ICoreInteractable.Interact()
        {
            try
            {
                Interact();
                errorUtility = ErrorUtility.Success(ErrorType.CoreInteractableInteract);
            }
            catch (Exception ex) { errorUtility = ErrorUtility.Error(ErrorType.CoreInteractableInteract, ex.Message, ex.StackTrace); throw ex; }
        }
        void ICoreInteractable.ShowInteraction()
        {
            try
            {
                ShowInteraction();
                errorUtility = ErrorUtility.Success(ErrorType.CoreInteractableShowInteraction);
            }
            catch (Exception ex) { errorUtility = ErrorUtility.Error(ErrorType.CoreInteractableShowInteraction, ex.Message, ex.StackTrace); throw ex; }
        }
        void ICoreInteractable.HideInteraction()
        {
            try
            {
                HideInteraction();
                errorUtility = ErrorUtility.Success(ErrorType.CoreInteractableHideInteraction);
            }
            catch (Exception ex) { errorUtility = ErrorUtility.Error(ErrorType.CoreInteractableHideInteraction, ex.Message, ex.StackTrace); throw ex; }
        }
        GameObject ICoreInteractable.GetGameObject() => this.gameObject;
        #endregion
    }
}
