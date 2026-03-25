using CoreInteractables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractableController : MonoBehaviour
{
    private ICoreInteractable _coreInteractableController;
    private Button _button; 
    void Start()
    {
        _coreInteractableController = GetComponentInParent<CoreInteractableController>();
        _button = GetComponent<Button>();
        _button.GetComponentInChildren<TextMeshProUGUI>().text = _coreInteractableController.GetButtonActionName();
        _button.onClick.AddListener(_coreInteractableController.Interact);
    }
}