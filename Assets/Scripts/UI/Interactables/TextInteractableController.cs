using CoreInteractables;
using TMPro;
using UnityEngine;

public class TextInteractableController : MonoBehaviour
{
    private ICoreInteractable _coreInteractableController;
    private TextMeshProUGUI _textTMP;
    void Start()
    {
        _coreInteractableController = GetComponentInParent<CoreInteractableController>();
        _textTMP = GetComponentInChildren<TextMeshProUGUI>();
        _textTMP.text = _coreInteractableController.GetInteractableName();
    }
}
