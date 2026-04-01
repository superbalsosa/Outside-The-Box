using Audio;
using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private SoundData hoverSound;
    [SerializeField] private SoundData clickSound;
    private ISoundManager soundManager;

    void Start()
    {
        soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        soundManager.CreateSound().WithSoundData(hoverSound).Play();
        if (!targetObject.IsUnityNull()) targetObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!targetObject.IsUnityNull()) targetObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        soundManager.CreateSound().WithSoundData(clickSound).Play();
        if (!targetObject.IsUnityNull()) targetObject.SetActive(false);
    }
}
