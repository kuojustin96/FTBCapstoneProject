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
    public SoundEffectManager sfm;

    public float fadeTime = 1f;

    [Header("Play Game")]
    public CanvasGroup playCanvas;

    [Header("Intructions")]
    public CanvasGroup instructionCanvas;

    [Header("Options")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resoDropdown;
    public TMP_InputField mouseSensInputField;
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

    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    bool preparing = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        Initialize();

        //Chayanne's Preference saving code
        bool firstTime = PlayerPrefs.GetInt("FirstRun") == 0;

        //Set defaults on first run
        if (firstTime)
        {
            Debug.Log("First run!");
            PlayerPrefs.SetInt("FirstRun", 1);

            PlayerPrefs.SetFloat("MouseSensitivity", 3.0f);

            PlayerPrefs.SetInt("QualityLevel", 3);

            PlayerPrefs.SetFloat("MasterVolume", 1);
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.SetFloat("SFXVolume", 1);

            PlayerPrefs.SetInt("FullScreen", 1);

            int width = Screen.width;
            int height = Screen.height;
            PlayerPrefs.SetInt("ScreenWidth", width);
            PlayerPrefs.SetInt("ScreenHeight", height);

        }

        string currentRes = PlayerPrefs.GetInt("ScreenWidth") + " x " + PlayerPrefs.GetInt("ScreenHeight");
        Debug.Log("current Resolution: " + currentRes);
        //load settings
        int qualitySettings = PlayerPrefs.GetInt("QualityLevel");

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");

        float mouseSens = PlayerPrefs.GetFloat("MouseSensitivity");

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        int prefsWidth = PlayerPrefs.GetInt("ScreenWidth");
        int prefsHeight = PlayerPrefs.GetInt("ScreenHeight");
        bool isFullScreen = (PlayerPrefs.GetInt("FullScreen") == 1);
        fullscreenToggle.isOn = isFullScreen;

        SetDropdownValue(prefsWidth, prefsHeight);

        qualityDropdown.value = qualitySettings;

        //Set resoulution based on width and height
        Screen.SetResolution(prefsWidth, prefsHeight, isFullScreen);

        preparing = false;

    }

    private void Initialize()
    {
        DOTween.Init();

        FadeManager.instance.CanvasGroupOFF(playCanvas, false, false);
        FadeManager.instance.CanvasGroupOFF(creditsCanvas, false, false);
        creditsScrollArea.anchoredPosition = new Vector2(creditsScrollArea.anchoredPosition.x, scrollStartY);
        FadeManager.instance.CanvasGroupOFF(optionsCanvas, false, false);
        FadeManager.instance.CanvasGroupOFF(instructionCanvas, false, false);

        SceneManager.sceneLoaded += OnSceneLoaded;

        PopulateDropdown(resoDropdown);
    }

    public void SetDropdownValue(int prefsWidth, int prefsHeight)
    {
        //Set dropdown (Yes it's this fucking complicated)
        for (int i = 0; i < resolutions.Length; i++)
        {
            bool widthMatches = ((resolutions[i].width - prefsWidth) == 0);
            bool heightMatches = ((resolutions[i].height - prefsHeight) == 0);

            if (widthMatches && heightMatches)
            {
                resoDropdown.value = i;
                break;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

            CloseCurrentCanvas();
        }
    }

    public void PopulateDropdown(TMP_Dropdown dropdown)
    {
        resolutions = Screen.resolutions;

        List<string> resoOptions = new List<string>();
        for (int x = 0; x < resolutions.Length; x++)
        {
            string option = resolutions[x].ToString();
            resoOptions.Add(option);
        }
        dropdown.AddOptions(resoOptions);
        dropdown.RefreshShownValue();
    }

    public void EnterButtonHover(BaseEventData data)
    {
        sfm.PlaySFX("Sugar Pickup", Camera.main.gameObject, 0.4f, true);
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
            sfm.PlaySFX("Sugar Pickup", Camera.main.gameObject, 0.4f, true);
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
            sfm.PlaySFX("MouseClick", Camera.main.gameObject, 0.5f, true);
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
            sfm.PlaySFX("MouseClick", Camera.main.gameObject, 0.5f, true);
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
            sfm.PlaySFX("MouseClick", Camera.main.gameObject, 0.5f, true);
            if (currentPanel != creditsCanvas)
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
        currentPanel = null;
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

    #region Intructions
    public void ToggleInstructions()
    {
        if (!inButtonTransition)
        {
            sfm.PlaySFX("MouseClick", Camera.main.gameObject, 0.5f, true);
            if (currentPanel != instructionCanvas)
                CloseCurrentCanvas();

            StartCoroutine(c_ToggleInstructions());
        }
    }

    private IEnumerator c_ToggleInstructions()
    {
        inButtonTransition = true;

        if (instructionCanvas.alpha == 0)
        {
            FadeManager.instance.FadeIn(instructionCanvas, fadeTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + fadeTime)
                yield return null;

            FadeManager.instance.CanvasGroupON(instructionCanvas, true, true);
            currentPanel = instructionCanvas;
        }
        else
        {
            FadeManager.instance.FadeOut(instructionCanvas, fadeTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + fadeTime)
                yield return null;

            FadeManager.instance.CanvasGroupOFF(instructionCanvas, false, false);
            currentPanel = null;
        }

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
        else if (currentPanel == instructionCanvas)
            ToggleInstructions();
        else
            TogglePlay();
    }

    public void SetMouseSensitivity()
    {
        float sens = float.Parse(mouseSensInputField.text);
        PlayerPrefs.SetFloat("MouseSensitivity", sens);
    }

    public void QuitGame()
    {
        sfm.PlaySFX("MouseClick", Camera.main.gameObject, 0.4f, true);
        Application.Quit();
    }

    public void ExitToMenu()
    {
        sfm.PlaySFX("MouseClick", Camera.main.gameObject, 0.4f, true);
        SceneManager.LoadScene(0);
    }

    public void SetMasterVolume(float volume)
    {
        if (volume <= muteVol)
            volume = -80f;

        audMixer.SetFloat("MasterVolume", volume);
        masterSlider.value = volume;
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);

    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= muteVol)
            volume = -80f;

        audMixer.SetFloat("MusicVolume", volume);
        musicSlider.value = volume;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= muteVol)
            volume = -80f;

        audMixer.SetFloat("SFXVolume", volume);
        sfxSlider.value = volume;

        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

    }

    public void SetGameQuality()
    {
        int qualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityDropdown.value = qualityIndex;

        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetGameQuality(TMP_Dropdown qualityDropdown)
    {
        int qualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetFullscreen()
    {
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;

        if (isFullscreen)
        {
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }
    }

    public void SetFullscreen(Toggle fullscreenToggle)
    {
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;

        if (isFullscreen)
        {
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }
    }

    public void SetResolution()
    {
        if (preparing)
            return;

        int resoIndex = resoDropdown.value;
        Resolution resolution = resolutions[resoIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ScreenWidth", resolution.width);
        PlayerPrefs.SetInt("ScreenHeight", resolution.height);
    }



    public void SetResolution(TMP_Dropdown resoDropdown)
    {
        int resoIndex = resoDropdown.value;
        Resolution resolution = resolutions[resoIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ScreenWidth", resolution.width);
        PlayerPrefs.SetInt("ScreenHeight", resolution.height);
    }

}
