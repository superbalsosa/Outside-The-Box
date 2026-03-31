using DependencyInjection;
using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenumanager : MonoBehaviour, IPauseMenuManager
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    private Dictionary<AudioSource, float> MasterAudio;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameplayMenu;

    IEventSystem eventManager;

    private void Awake()
    {
        InterfaceDependencyInjector.Instance.Register<IPauseMenuManager>(() => this);
    }

    private void Start()
    {
        eventManager = InterfaceDependencyInjector.Instance.Resolve<IEventSystem>();
        masterVolumeSlider.onValueChanged.AddListener(OnVolumeChangedMaster);
        sfxVolumeSlider.onValueChanged.AddListener(OnVolumeChangedSFX);
        bgmVolumeSlider.onValueChanged.AddListener(OnVolumeChangedBgm);
        SetAllVolumensToSameValue();
        InitAudios();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !eventManager.GetCurrentEvent().Equals(EventType.InteractableControlTable))
        {
            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                gameplayMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                gameplayMenu.SetActive(false);
            }
        }
    }
    private void InitAudios()
    {
        MasterAudio = new Dictionary<AudioSource, float>();

        var auxAudios = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var audio in auxAudios)
        {
            MasterAudio.Add(audio, audio.volume);
        }
    }
    private void OnVolumeChangedMaster(float value)
    {
        SetVolume("Master", value);
    }
    private void OnVolumeChangedSFX(float value)
    {
        SetVolume("SFX", value);
    }
    private void OnVolumeChangedBgm(float value)
    {
        SetVolume("BGM", value);
    }
    private void SetVolume(string parameterName, float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, dB);

        PlayerPrefs.SetFloat(parameterName, value);
    }

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnVolumeChangedMaster);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnVolumeChangedSFX);
    }

    private void SetAllVolumensToSameValue()
    {
        SetVolume("Master", PlayerPrefs.GetFloat("Master"));
        masterVolumeSlider.value = PlayerPrefs.GetFloat("Master");
        SetVolume("SFX", PlayerPrefs.GetFloat("SFX"));
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFX");
        SetVolume("BGM", PlayerPrefs.GetFloat("BGM"));
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGM");
    }

    public void LoadMenu()
    {      
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
    public GameObject PauseGameObject()
    {
        return pauseMenu;
    }

    public void DissableAllUI()
    {
        pauseMenu.SetActive(false);
        gameplayMenu.SetActive(false);
    }

    public void EnableGameplayUI()
    {
        gameplayMenu.SetActive(true);
    }

}

public interface IPauseMenuManager
{
    GameObject PauseGameObject();
    void DissableAllUI();
    void EnableGameplayUI();
}