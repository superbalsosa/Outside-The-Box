using Audio.Data;
using Audio.Interfaces;
using DependencyInjection;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] float timeToChangeSceneAfterCommand;
    [SerializeField] string nextSceneName;
    [SerializeField] float timeToEnableButtons;

    [Header("Sound")]
    [SerializeField] SoundData clickSoundData;
    [SerializeField] SoundData hoverSoundData;
    [SerializeField] AudioSource bgmAudio;

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

    }
    private void Update()
    {
        if (bgmAudio != null)
        {
            if (isDecreasingVolume)
            {
                bgmAudio.volume -= (bgmVolumeBase) * Time.deltaTime * (1 / (timeToChangeSceneAfterCommand));
            }

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
