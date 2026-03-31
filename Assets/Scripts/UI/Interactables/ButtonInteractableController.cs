using Audio;
using Audio.Data;
using Audio.Interfaces;
using CoreInteractables;
using DependencyInjection;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractableController : MonoBehaviour
{
    [SerializeField] private SoundData _soundData;
    private ICoreInteractable _coreInteractableController;
    ISoundManager _soundManager;
    private Button _button;
    private void OnEnable()
    {
        if (_soundManager.IsUnityNull())
        {
            _soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();
        }
        _soundManager.CreateSound().WithSoundData(_soundData).Play();
    }
    void Start()
    {
        _coreInteractableController = GetComponentInParent<CoreInteractableController>();
        _button = GetComponent<Button>();
        _button.GetComponentInChildren<TextMeshProUGUI>().text = _coreInteractableController.GetButtonActionName();
        _button.onClick.AddListener(_coreInteractableController.Interact);

    }
    void OnButtonClick()
    {
        StartCoroutine(DelayedActionRoutine());
    }

    IEnumerator DelayedActionRoutine()
    {
        yield return new WaitForSeconds(1f); 
    }

}