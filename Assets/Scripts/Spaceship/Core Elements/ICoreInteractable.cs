using UnityEngine;
using Utilities.Error;
public interface ICoreInteractable

{
    public void Interact();
    public void ShowInteraction();
    public void HideInteraction();
    public GameObject GetGameObject();
}