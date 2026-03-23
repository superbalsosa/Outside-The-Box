using System.Collections.Generic;
using DependencyInjection;
using UnityEngine;
using System;

namespace CoreInteractables
{
    public class CoreInteractableController : MonoBehaviour, ICoreInteractable
    {
        #region VARIABLES
        [SerializeField] private CoreInteractableSO interactableData;
        private List<Renderer> renderers = new List<Renderer>();
        private IErrorManager errorManager;
        #endregion

        #region UNITY_METHODS
        private void Start()
        {
            errorManager = InterfaceDependencyInjector.Instance.Resolve<IErrorManager>();
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
            }
            catch (Exception ex) { errorManager.WriteError(ErrorType.CoreInteractable_Interact, ex.Message, ex.StackTrace); }
        }
        void ICoreInteractable.ShowInteraction()
        {
            try
            {
                ShowInteraction();
            }
            catch (Exception ex) { errorManager.WriteError(ErrorType.CoreInteractable_ShowInteraction, ex.Message, ex.StackTrace); }
        }
        void ICoreInteractable.HideInteraction()
        {
            try
            {
                HideInteraction();
            }
            catch (Exception ex) { errorManager.WriteError(ErrorType.CoreInteractable_HideInteraction, ex.Message, ex.StackTrace); }
        }
        GameObject ICoreInteractable.GetGameObject() => this.gameObject;
        #endregion
    }
}
