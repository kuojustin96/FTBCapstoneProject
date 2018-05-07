using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour {

    public static MainMenuManager instance;

    public float fadeTime = 1f;

    [Header("Play Game")]
    public CanvasGroup playCanvas;

    [Header("Options")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resoDropdown;
    public Toggle fullscreenToggle;
    public CanvasGroup optionsCanvas;
    public AudioMixer audMixer;
    public float muteVol = -45f;

    [Header("Credits")]
    public CanvasGroup creditsCanvas;
    public RectTransform creditsScrollArea;
    public float scrollTime = 30f;
    public float scrollStartY = -900;
    public float scrollEndY = 3000f;
    private bool isCreditsPlaying = false;
    private Coroutine creditsCoroutine;
    private Tweener currentTween;

    private CanvasGroup currentPanel;
    private Resolution[] resolutions;
    private bool inButtonTransition = false;
    private GameObject buttonHovered;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        DOTween.Init();

        FadeManager.instance.CanvasGroupOFF(playCanvas, false, false);
        FadeManager.instance.CanvasGroupOFF(creditsCanvas, false, false);
        creditsScrollArea.anchoredPosition = new Vector2(creditsScrollArea.anchoredPosition.x, scrollStartY);
        FadeManager.instance.CanvasGroupOFF(optionsCanvas, false, false);

        PopulateDropdown(resoDropdown);
        fullscreenToggle.isOn = Screen.fullScreen;
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    public void PopulateDropdown(TMP_Dropdown dropdown)
    {
        resolutions = Screen.resolutions;

        List<string> resoOptions = new List<string>();
        int currentIndex = 0;
        for (int x = 0; x < resolutions.Length; x++)
        {
            string option = resolutions[x].width + " x " + resolutions[x].height;
            resoOptions.Add(option);

            if (resolutions[x].width == Screen.currentResolution.width && resolutions[x].height == Screen.currentResolution.height)
            {
                currentIndex = x;
                Debug.Log(currentIndex);
            }
        }
        dropdown.AddOptions(resoOptions);
        dropdown.value = currentIndex;
        dropdown.RefreshShownValue();
    }

    public void EnterButtonHover(BaseEventData data)
    {
        PointerEventData d = data as PointerEventData;
        GameObject g = d.pointerEnter;
        buttonHovered = g;

        g.transform.localScale *= 1.2f;
        g.transform.localEulerAngles = new Vector3(0, 0, 3);
    }

    public void ExitButtonHover()
    {
        if(buttonHovered != null)
        {
            buttonHovered.transform.localScale = Vector2.one;
            buttonHovered.transform.localEulerAngles = Vector3.zero;
            buttonHovered = null;
        }
    }

    #region Options
    public void ToggleOptions()
    {
        if (!inButtonTransition)
        {
            if (currentPanel != optionsCanvas)
                CloseCurrentCanvas();

            StartCoroutine(c_ToggleOptions());
        }
    }

    private IEnumerator c_ToggleOptions()
    {
        inButtonTransition = true;

        if (optionsCanvas.alpha == 0)
        {
            FadeManager.instance.FadeIn(optionsCanvas, fadeTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + fadeTime)
                yield return null;

            FadeManager.instance.CanvasGroupON(optionsCanvas, true, true);
            currentPanel = optionsCanvas;
        }
        else
        {
            FadeManager.instance.FadeOut(optionsCanvas, fadeTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + fadeTime)
                yield return null;

            FadeManager.instance.CanvasGroupOFF(optionsCanvas, false, false);
            currentPanel = null;
        }

        inButtonTransition = false;
    }
    #endregion

    #region Play Game
    public void TogglePlay()
    {
        if (!inButtonTransition)
        {
            if (currentPanel != playCanvas)
                CloseCurrentCanvas();

            StartCoroutine(c_TogglePlay());
        }
    }

    private IEnumerator c_TogglePlay()
    {
        inButtonTransition = true;

        if (playCanvas.alpha == 0)
        {
            FadeManager.instance.FadeIn(playCanvas, fadeTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + fadeTime)
                yield return null;

            FadeManager.instance.CanvasGroupON(playCanvas, true, true);
            currentPanel = playCanvas;
        }
        else
        {
            FadeManager.instance.FadeOut(playCanvas, fadeTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + fadeTime)
                yield return null;

            FadeManager.instance.CanvasGroupOFF(playCanvas, false, false);
            currentPanel = null;
        }

        inButtonTransition = false;
    }
    #endregion

    #region Credits
    public void ToggleCredits()
    {
        //Might need a general button check to make sure player isn't spam clicking a button
        if (!inButtonTransition)
        {
            if(currentPanel != creditsCanvas)
                CloseCurrentCanvas();

            if (!isCreditsPlaying)
            {
                creditsCoroutine = StartCoroutine(c_OpenCredits());
                MusicManager.instance.SwapMainTracks("OpeningMusic", 0.75f, 1);
            }
            else
            {
                StartCoroutine(c_CloseCredits());
                MusicManager.instance.SwapMainTracks("PregameLobby", 1, 1);
            }
        }
    }

    private IEnumerator c_OpenCredits()
    {
        isCreditsPlaying = true;
        inButtonTransition = true;

        FadeManager.instance.FadeIn(creditsCanvas, fadeTime);
        float saveTime = Time.time;
        while (Time.time < saveTime + fadeTime)
            yield return null;

        inButtonTransition = false;
        currentPanel = creditsCanvas;

        currentTween = creditsScrollArea.DOAnchorPosY(scrollEndY, scrollTime).SetEase(Ease.Linear);
        saveTime = Time.time;
        while (Time.time < saveTime + scrollTime)
            yield return null;

        FadeManager.instance.FadeOut(creditsCanvas, fadeTime);
        saveTime = Time.time;
        while (Time.time < saveTime + fadeTime)
            yield return null;

        MusicManager.instance.SwapMainTracks("PregameLobby", 1, 1);
        isCreditsPlaying = false;
        creditsScrollArea.anchoredPosition = new Vector2(creditsScrollArea.anchoredPosition.x, scrollStartY);
    }
    
    private IEnumerator c_CloseCredits()
    {
        inButtonTransition = true;

        FadeManager.instance.FadeOut(creditsCanvas, fadeTime);
        float saveTime = Time.time;
        while (Time.time < saveTime + fadeTime)
            yield return null;

        StopCoroutine(creditsCoroutine);
        creditsCoroutine = null;
        currentTween.Kill();
        currentTween = null;
        isCreditsPlaying = false;
        creditsScrollArea.anchoredPosition = new Vector2(creditsScrollArea.anchoredPosition.x, scrollStartY);

        currentPanel = null;

        inButtonTransition = false;
    }
    #endregion

    private void CloseCurrentCanvas()
    {
        if (currentPanel == null)
            return;

        if (currentPanel == optionsCanvas)
            ToggleOptions();
        else if (currentPanel == creditsCanvas)
            ToggleCredits();
        else
            TogglePlay();
    }

    public void StartGame()
    {
        //SceneManager.LoadScene("MainScene");
        //Convert to Host / Join game
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetMasterVolume(float volume)
    {
        if (volume <= muteVol)
            volume = -80f;

        audMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= muteVol)
            volume = -80f;

        audMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= muteVol)
            volume = -80f;

        audMixer.SetFloat("SFXVolume", volume);
    }

    public void SetGameQuality()
    {
        int qualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetGameQuality(TMP_Dropdown qualityDropdown)
    {
        int qualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen()
    {
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;
    }

    public void SetFullscreen(Toggle fullscreenToggle)
    {
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution()
    {
        int resoIndex = resoDropdown.value;
        Resolution resolution = resolutions[resoIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetResolution(TMP_Dropdown resoDropdown)
    {
        int resoIndex = resoDropdown.value;
        Resolution resolution = resolutions[resoIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
