using UnityEngine;
public interface ICoreInteractable

{
    public void Interact();
    public void ShowInteraction();
    public void HideInteraction();
    public GameObject GetGameObject();
    public string GetButtonActionName();
    public string GetInteractableName();
}