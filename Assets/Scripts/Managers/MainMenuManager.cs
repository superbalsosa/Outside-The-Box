using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] float timeToChangeSceneAfterCommand;
    [SerializeField] string nextSceneName;
    [SerializeField] float timeToEnableButtons;

    [Header("Sound")]
    [SerializeField] SoundData clickSoundData;
    [SerializeField] SoundData hoverSoundData;


    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    private Dictionary<AudioSource, float> MasterAudio;

    //[Header("Cursor")]
    //[SerializeField] Texture2D cursorTexture;

    [SerializeField] private GameObject spaceShip;
    [SerializeField] private GameObject warpDrive;

    [Header("Panel Settings")]
    [SerializeField] MenuPanel activePanel;
    [SerializeField] MenuPanel panel_1;
    [SerializeField] MenuPanel panel_2;
    [SerializeField] MenuPanel panel_3;
    [SerializeField] TextMeshProUGUI PanelTitleText;

    private bool isTransitioning;
    private Coroutine transitionCoroutine;

    private bool buttonsAllowed = false;
    private bool isDecreasingVolume = false;

    private float bgmVolumeBase;

    ISoundManager soundManager;


    private void Awake()
    {
        //soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();      
    }
    void Start()
    {
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        activePanel = panel_1;
        buttonsAllowed = true;
        isTransitioning = false;
        //bgmVolumeBase = bgmAudio.volume;

        masterVolumeSlider.onValueChanged.AddListener(OnVolumeChangedMaster);
        sfxVolumeSlider.onValueChanged.AddListener(OnVolumeChangedSFX);
        bgmVolumeSlider.onValueChanged.AddListener(OnVolumeChangedBgm);
        SetAllVolumensToSameValue();
        InitAudios();

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

    public void ChangeToNextLevel()
    {
        if (!buttonsAllowed) return;
        //AllowButtons(false);
        //ClikBehaviour();
        isDecreasingVolume = true;
        warpDrive.SetActive(true);
        activePanel.gameObject.SetActive(false);
        PanelTitleText.gameObject.SetActive(false);

        StartCoroutine(ChangeNextLevel(timeToChangeSceneAfterCommand));
    }
    public void QuitGame()
    {
        if (!buttonsAllowed) return;
        AllowButtons(false);
        //ClikBehaviour();

        StartCoroutine(ExitGame(timeToChangeSceneAfterCommand));
    }


    public void ChangeActivePanel(MenuPanel panel)
    {
        if (isTransitioning) return;


        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionRoutine(panel));
    }
    private IEnumerator TransitionRoutine(MenuPanel panel)
    {
        isTransitioning = true;
        buttonsAllowed = false;

        activePanel.gameObject.SetActive(false);
        
        PanelTitleText.text = panel.panelTitle;

        yield return new WaitForSeconds(1f); 

        activePanel = panel;


        activePanel.gameObject.SetActive(true);

        buttonsAllowed = true;
        isTransitioning = false;
    }

    private void ClikBehaviour()
    {

        soundManager.CreateSound()
            .WithSoundData(clickSoundData)
            .WithRandomPitch()
            .Play();
        buttonsAllowed = false;
    }

    public void HoverBehaviour()
    {
        //if (!buttonsAllowed) return;
        soundManager.CreateSound()
            .WithSoundData(hoverSoundData)
            .WithRandomPitch()
            .Play();
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
        Debug.Log("BGM slider: " + value);
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

    private IEnumerator AllowButtons(bool isAllowed, float seconds = 0f)
    {
        yield return new WaitForSeconds(seconds);
        buttonsAllowed = true;
    }
    private IEnumerator ExitGame(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Application.Quit();
    }
    private IEnumerator ChangeNextLevel(float seconds)
    {
        float time = 0;

        while (time < seconds - 1.5f)
        {
            spaceShip.transform.position = Vector3.Lerp(spaceShip.transform.position, Vector3.zero, time/seconds);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }
  
}
