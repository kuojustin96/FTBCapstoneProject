using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public float fadeTime = 1f;
    public CanvasGroup mainMenu;
    public CanvasGroup gameSettings;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resoDropdown;
    public Toggle fullscreenToggle;
    private CanvasGroup currentPanel;

    private Resolution[] resolutions;

	// Use this for initialization
	void Start () {
        FadeManager.instance.CanvasGroupOFF(gameSettings, true);
        FadeManager.instance.CanvasGroupON(mainMenu, true);

        resolutions = Screen.resolutions;

        List<string> resoOptions = new List<string>();
        int currentIndex = 0;
        for(int x = 0; x < resolutions.Length; x++)
        {
            string option = resolutions[x].width + " x " + resolutions[x].height;
            resoOptions.Add(option);

            if (resolutions[x].width == Screen.currentResolution.width && resolutions[x].height == Screen.currentResolution.height)
                currentIndex = x;
        }
        resoDropdown.AddOptions(resoOptions);
        resoDropdown.value = currentIndex;
        resoDropdown.RefreshShownValue();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BackToMenu()
    {
        FadeManager.instance.ChangeMenuPanels(currentPanel, mainMenu, fadeTime);
        currentPanel = mainMenu;
    }

    public void OpenGameSettings()
    {
        FadeManager.instance.ChangeMenuPanels(mainMenu, gameSettings, fadeTime);
        currentPanel = gameSettings;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMasterVolume(float volume)
    {
        Debug.Log("Set Master Volume");
    }

    public void SetGameQuality()
    {
        int qualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen()
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
}
