using System.Collections.Generic;
using DependencyInjection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace CoreInteractables
{
    public class CoreInteractableController : MonoBehaviour, ICoreInteractable
    {
        #region VARIABLES
        [SerializeField] private CoreInteractableSO interactableData;
        private Canvas canvasButtonInteractive;
        private List<Renderer> renderers = new List<Renderer>();
        //private IErrorManager errorManager;
        private IEventSystem eventManager;
        private GameObject alertUI;
        #endregion

        #region UNITY_METHODS
        private void Start()
        {
            canvasButtonInteractive = GetComponentInChildren<Canvas>(true);
            if (interactableData.IsAlertable) alertUI = GetComponentInChildren<AlertUIController>(true).gameObject;
            //errorManager = InterfaceDependencyInjector.Instance.Resolve<IErrorManager>();
            eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
            InitOutlines();
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitOutlines()
        {
            var childrenRenderers = GetComponentsInChildren<Renderer>();

            foreach (var childRender in childrenRenderers)
            {
                if (childRender.material.shader.name.Equals("Outline_Built_In"))
                {
                    renderers.Add(childRender);
                }
            }
            ResetState();
        }
        private void ResetState() => HideInteraction();
        private void Interact()
        {
            eventManager.SetEvent(interactableData.eventToTrigger, true);
        }
        private void ShowInteraction()
        {
            if (interactableData.IsAlertable) alertUI.gameObject.SetActive(false);
            canvasButtonInteractive.gameObject.SetActive(true);

            foreach (var render in renderers)
            {
                render.material.SetFloat("_OutlineWidth", interactableData.OutlineHover);
            }
        }
        private void HideInteraction()
        {
            canvasButtonInteractive.gameObject.SetActive(false);
            if (interactableData.IsAlertable) alertUI.gameObject.SetActive(true);

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
            catch (Exception ex) {
                //errorManager.WriteError(ErrorType.CoreInteractable_Interact, ex.Message, ex.StackTrace);
                Debug.LogException(ex);
            }
        }
        void ICoreInteractable.ShowInteraction()
        {
            try
            {
                ShowInteraction();
            }
            catch (Exception ex) { 
                //errorManager.WriteError(ErrorType.CoreInteractable_ShowInteraction, ex.Message, ex.StackTrace);
                Debug.LogException(ex);
            }
        }
        void ICoreInteractable.HideInteraction()
        {
            try
            {
                HideInteraction();
            }
            catch (Exception ex) { 
                //errorManager.WriteError(ErrorType.CoreInteractable_HideInteraction, ex.Message, ex.StackTrace);
                Debug.LogException(ex);
            }
        }
        GameObject ICoreInteractable.GetGameObject() => this.gameObject;
        string ICoreInteractable.GetButtonActionName() => interactableData.ButtonActionName;
        //In case of ControlTable, the interactable name should change in base of the StateMachine that's not implemented yet.
        string ICoreInteractable.GetInteractableName() => interactableData.InteractableName;

        #endregion
    }
}
