using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Networking;
using DG.Tweening;

public enum TickerBehaviors
{
    None = 0,
    Emotes = 1,
    TickerText = 2,
}

public class UIController : NetworkBehaviour {

    private PlayerClass player;
    private NetworkSoundController nsc;
    private NetworkParticleController netparticle;
    private jkuo.net_PlayerController npc;
    private StatManager sm;
    public Texture2D cursorTexture;
    public CanvasGroup FaderPanel;

    [System.Serializable]
    public class ItemTexture
    {
        public string name;
        public Texture texture;
    }

    public GameObject UICanvas;

    [Header("Crafting UI")]
    public ItemTexture[] ItemTextures;
    private Dictionary<string, Texture> TextureDict = new Dictionary<string, Texture>();

    public RawImage[] bagLines;

    public float CraftItemTime = 3f;

    public Texture NoItemTexture;
    public CanvasGroup OpenCraftingUI;
    public CanvasGroup CraftingUI;
    private RectTransform CraftingUIRect;
    public RawImage CraftingItemUI;
    private RectTransform CraftingItemUIRect;
    public Image CraftingItemFill;
    public TextMeshProUGUI CraftingItemPercentage;
    private bool inCraftMenu = false;
    

    [Header("Ingame UI")]
    public CanvasGroup IngameItemBackgroundUI;
    private RectTransform IngameItemRect;
    public RawImage IngameItemUI;
    private Texture lastItemCrafted;
    private RectTransform IngameItemUIRect;
    public TextMeshProUGUI BackpackScore;
    public TextMeshProUGUI StashScore;

    public RectTransform[] ItemButtons;
    private Vector2 itemButtonSize;

    public TextMeshProUGUI gameTime;
    public TextMeshProUGUI milisecondGameTime;

    [Header("Pause UI")]
    private MainMenuManager mmm;
    public CanvasGroup pauseCG;
    public Slider masterVolSlider;
    public Slider musicVolSlider;
    public Slider sfxVolSlider;
    public TMP_InputField mouseSensInput;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle windowedToggle;
    public Button exitToMenu;
    public Button exitToDesktop;

    [Header("Ticker")]
    public RawImage tickerBackgroud;
    public float tickerLerpTime = 10f;
    public float tickerTextLerpTime = 10f;
    public GameObject emoteUI;
    private CanvasGroup emoteUICG;
    public RectTransform tickerTextUI;
    private CanvasGroup tickerTextUICG;
    public TextMeshProUGUI tickerText;
    private Vector2 tickerEnabledPos;
    private Vector2 tickerDisabledPos;
    public bool tickerEnabled { get; protected set; }
    private bool tickerTextPlaying = false;
    private string currentMessage;
    private Coroutine tickerCoroutine;
    private Tweener tickerTween;

    private Vector2 origIngameUIPos;
    private Vector2 origIngameUIScale;
    private Vector2 origIngameItemUIPos;
    private Vector2 origIngameItemUIScale;

    private int currentSugar = 0;
    private int maxBackpackScore = 10;
    private int lineDiviser = 2;

    [Header("Transition Between UIs")]
    public float UIShiftTime = 1f;

    private craftingInput ci;
    private Coroutine craftingCoroutine;
    private RectTransform lastHoveredButton;
    private Image lastHoveredImage;
    private Color saveButtonColor;

	private bool craftingItem = false;

	// Use this for initialization
	void Start () {
        DOTween.Init();
        SetUpTicker();

        nsc = GetComponent<NetworkSoundController>();
        netparticle = GetComponent<NetworkParticleController>();
        emoteUICG = emoteUI.GetComponent<CanvasGroup>();
        tickerTextUICG = tickerTextUI.GetComponent<CanvasGroup>();
        emoteUICG.alpha = 0f;
        tickerTextUICG.alpha = 0f;
        tickerTextUI.anchoredPosition = new Vector2(2000, tickerTextUI.anchoredPosition.y);
    }

    public void SetUpVariables(PlayerClass player)
    {
        sm = StatManager.instance;
        sm.SetUIController(this);
        npc = GetComponent<jkuo.net_PlayerController>();

        IngameItemRect = IngameItemBackgroundUI.GetComponent<RectTransform>();
        origIngameUIScale = IngameItemRect.sizeDelta;
        origIngameUIPos = IngameItemRect.position;

        CraftingUIRect = CraftingUI.GetComponent<RectTransform>();

        IngameItemUIRect = IngameItemUI.rectTransform;
        origIngameItemUIPos = IngameItemUIRect.localPosition;
        origIngameItemUIScale = IngameItemUIRect.sizeDelta;

        CraftingItemUIRect = CraftingItemUI.rectTransform;

        CraftingItemFill.fillAmount = 0f;
        CraftingItemPercentage.text = "0%";
        CraftingItemFill.gameObject.SetActive(false);

        foreach(ItemTexture it in ItemTextures)
        {
            if(!TextureDict.ContainsKey(it.name))
                TextureDict.Add(it.name, it.texture);
        }

        itemButtonSize = ItemButtons[0].sizeDelta;
        foreach (RectTransform rt in ItemButtons)
            rt.sizeDelta = Vector2.zero;

        CanvasOFF(OpenCraftingUI);
        CanvasOFF(CraftingUI);
        CanvasON(IngameItemBackgroundUI, false);

        UpdateMaxBackpackScore(maxBackpackScore);

        OpenCraftingUI.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Vector2 hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);
        Cursor.visible = true;

        FaderPanel.alpha = 1f;
        FadeManager.instance.FadeOut(FaderPanel, 1f);

        this.player = player;
        ci = player.playerGO.GetComponent<craftingInput>();

        //Set Up Pause Menu
        mmm = MainMenuManager.instance;
        FadeManager.instance.CanvasGroupOFF(pauseCG, false, false);

        //Set Listeners
        masterVolSlider.onValueChanged.AddListener(delegate { mmm.SetMasterVolume(masterVolSlider.value); });
        musicVolSlider.onValueChanged.AddListener(delegate { mmm.SetMusicVolume(musicVolSlider.value); });
        sfxVolSlider.onValueChanged.AddListener(delegate { mmm.SetSFXVolume(sfxVolSlider.value); });
        windowedToggle.onValueChanged.AddListener(delegate { mmm.SetFullscreen(windowedToggle); });
        resolutionDropdown.onValueChanged.AddListener(delegate { mmm.SetResolution(resolutionDropdown); });
        qualityDropdown.onValueChanged.AddListener(delegate { mmm.SetGameQuality(qualityDropdown); });
        exitToMenu.onClick.AddListener(delegate { mmm.ExitToMenu(); });
        exitToDesktop.onClick.AddListener(delegate { mmm.QuitGame(); });

        EventTrigger desktopTrigger = exitToDesktop.GetComponent<EventTrigger>();
        EventTrigger menuTrigger = exitToMenu.GetComponent<EventTrigger>();

        EventTrigger.Entry onButtonEnter = new EventTrigger.Entry();
        onButtonEnter.eventID = EventTriggerType.PointerEnter;
        onButtonEnter.callback.AddListener((data) => { mmm.EnterButtonHover((BaseEventData)data); });
        desktopTrigger.triggers.Add(onButtonEnter);
        menuTrigger.triggers.Add(onButtonEnter);
        EventTrigger.Entry onButtonExit = new EventTrigger.Entry();
        onButtonExit.eventID = EventTriggerType.PointerExit;
        onButtonExit.callback.AddListener((data) => { mmm.ExitButtonHover(); });
        desktopTrigger.triggers.Add(onButtonExit);
        menuTrigger.triggers.Add(onButtonExit);

        float mouseSens = PlayerPrefs.GetFloat("MouseSensitivity");
        mouseSensInput.text = mouseSens.ToString();
        npc.lookSensitivity = mouseSens;

        float tempValue;
        mmm.audMixer.GetFloat("MasterVolume", out tempValue);
        masterVolSlider.value = tempValue;
        mmm.audMixer.GetFloat("MusicVolume", out tempValue);
        musicVolSlider.value = tempValue;
        mmm.audMixer.GetFloat("SFXVolume", out tempValue);
        sfxVolSlider.value = tempValue;

        windowedToggle.isOn = Screen.fullScreen;
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        mmm.PopulateDropdown(resolutionDropdown);

        int prefsWidth = PlayerPrefs.GetInt("ScreenWidth");
        int prefsHeight = PlayerPrefs.GetInt("ScreenHeight");
        bool isFullScreen = (PlayerPrefs.GetInt("FullScreen") == 1);
        mmm.SetDropdownValue(prefsWidth, prefsHeight);
        Screen.SetResolution(prefsWidth, prefsHeight, isFullScreen);
    }
	
	// Update is called once per frame
	void Update () {
        if (player == null || !isLocalPlayer)
            return;

        //Probably want to move this to OnTriggerEnter in sugar pickup
        if (player.showCraftingUI && !player.craftingUIOpen)
            CanvasON(OpenCraftingUI);

        //Probably want to move this to OnTriggerEnter in sugar pickup
        if (!player.showCraftingUI)
        {
            if(OpenCraftingUI.alpha == 1f)
                CanvasOFF(OpenCraftingUI);

            if (CraftingUI.alpha == 1f)
                CanvasOFF(CraftingUI);
        }


        if (Input.GetKeyDown(KeyCode.F) && player.showCraftingUI)
            ToggleCraftingUI();

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();

        UpdateCursorLock();

        //Update Game Timer
        gameTime.text = sm.GetCurrentTimeLeft();
        milisecondGameTime.text = sm.GetCurrentMiliseconds();
    }

    public void SetMouseSensitivity()
    {
        float sens = float.Parse(mouseSensInput.text);
        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        npc.lookSensitivity = sens;
    }

    private void TogglePauseMenu()
    {
        if (!player.playerPaused)
        {
            //SoundEffectManager.instance.StopAllSFX();
            player.playerPaused = true;
            FadeManager.instance.CanvasGroupON(pauseCG, true, true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            player.playerPaused = false;
            FadeManager.instance.CanvasGroupOFF(pauseCG, false, false);
            Cursor.lockState = CursorLockMode.Locked;
            
            if(npc.pausedGravity != null)
            {
                StopCoroutine(npc.pausedGravity);
                npc.pausedGravity = null;
            }
        }
    }

    private void UpdateCursorLock()
    {
        if (Input.GetMouseButton(0) && Cursor.lockState == CursorLockMode.None && !inCraftMenu && !player.playerPaused)
            Cursor.lockState = CursorLockMode.Locked;
    }

    #region Enable/Disable Crafting UI
    private void ToggleCraftingUI()
    {
        if (CraftingUI.alpha == 0f)
            StartCoroutine(ShowCraftingUI());
        else if(CraftingUI.alpha == 1f)
            StartCoroutine(HideCraftingUI());
    }

    private IEnumerator ShowCraftingUI()
    {
        player.craftingUIOpen = true;
        CanvasOFF(OpenCraftingUI);

        IngameItemBackgroundUI.transform.DOMove(CraftingUI.transform.position, UIShiftTime);
        IngameItemUIRect.DOSizeDelta(CraftingItemUI.rectTransform.sizeDelta, UIShiftTime);
        IngameItemUIRect.DOAnchorPos(Vector2.zero, UIShiftTime);

        float saveTime = Time.time;
        while (Time.time < saveTime + UIShiftTime)
            yield return null;

        foreach (RectTransform rt in ItemButtons)
            rt.DOSizeDelta(itemButtonSize, UIShiftTime / 2);

        saveTime = Time.time;
        while (Time.time < saveTime + (UIShiftTime / 2))
            yield return null;

        CanvasOFF(IngameItemBackgroundUI);
        CanvasON(CraftingUI);

        Cursor.lockState = CursorLockMode.None;
        inCraftMenu = true;
        //Cursor.visible = true;
    }

    private IEnumerator HideCraftingUI()
    {
        player.craftingUIOpen = false;
        CanvasOFF(CraftingUI);
        CanvasON(IngameItemBackgroundUI);

        CraftingItemFill.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        inCraftMenu = false;
        //Cursor.visible = false;

        foreach (RectTransform rt in ItemButtons)
            rt.DOSizeDelta(Vector2.zero, UIShiftTime);

        float saveTime = Time.time;
        while (Time.time < saveTime + UIShiftTime)
            yield return null;

        IngameItemBackgroundUI.transform.DOMove(origIngameUIPos, UIShiftTime);
        IngameItemUIRect.DOSizeDelta(origIngameItemUIScale, UIShiftTime);
        IngameItemUIRect.DOAnchorPos(origIngameItemUIPos, UIShiftTime);

        saveTime = Time.time;
        while (Time.time < saveTime + UIShiftTime)
            yield return null;

        IngameItemBackgroundUI.transform.position = origIngameUIPos;
        IngameItemUIRect.sizeDelta = origIngameItemUIScale;
        IngameItemUIRect.transform.localPosition = origIngameItemUIPos;

        CanvasON(OpenCraftingUI);
    }

    public void CancelCrafting()
    {
        if(craftingCoroutine != null)
        {
            IngameItemUI.texture = lastItemCrafted;
            StopCoroutine(craftingCoroutine);
            StartCoroutine(HideCraftingUI());
            craftingCoroutine = null;
        }
    }
    #endregion

    #region Button Behaviors
    public void EnterHoverButton(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        lastHoveredButton = pointerData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>();
        lastHoveredButton.localScale = Vector3.one * 1.1f;

        lastHoveredImage = lastHoveredButton.GetComponent<Image>();
        saveButtonColor = lastHoveredImage.color;
        lastHoveredImage.color = lastHoveredImage.color.gamma;
    }

    public void ExitHoverButton()
    {
        if (lastHoveredButton == null)
            return;

        lastHoveredButton.localScale = Vector3.one;
        lastHoveredButton = null;

        lastHoveredImage.color = saveButtonColor;
        lastHoveredImage = null;
    }

    public void ClickAttackButton()
    {
		if(player.currentPlayerScore>0 && craftingItem == false)
            craftingCoroutine = StartCoroutine(CraftItemWaitTime("Attack"));
    }

    public void ClickDefenseButton()
    {
		if(player.currentPlayerScore>0 && craftingItem == false)
            craftingCoroutine = StartCoroutine(CraftItemWaitTime("Defense"));
    }

    public void ClickUtilityButton()
    {
		if(player.currentPlayerScore>0 && craftingItem == false)
            craftingCoroutine = StartCoroutine(CraftItemWaitTime("Utility"));
    }

    private IEnumerator CraftItemWaitTime(string itemType)
    {
        craftingItem = true;
        lastItemCrafted = IngameItemUI.texture;
        IngameItemUI.texture = NoItemTexture;
        CraftingItemUI.texture = NoItemTexture;
        CraftingItemFill.fillAmount = 0f;
        CraftingItemFill.gameObject.SetActive(true);
        SoundEffectManager.instance.PlaySFX("Crafting", gameObject);

        float time = Time.time;
        while(Time.time < time + CraftItemTime)
        {
            CraftingItemFill.fillAmount += Time.deltaTime / CraftItemTime;
            CraftingItemPercentage.text = Mathf.RoundToInt(CraftingItemFill.fillAmount * 100) + "%";
            yield return null;
        }

        switch (itemType)
        {
            case "Attack":
                ci.craftAttack();
                break;

            case "Defense":
                ci.craftDefense();
                break;

			case "Utility":
				ci.craftUtility ();
                break;
        }

        sm.UpdateStat(Stats.ItemsCrafted);
        MusicManager.instance.defaultGameplayTrack = "GroundFloorAttack";

        IngameItemUI.texture = TextureDict[player.currentItem.name];
        CraftingItemUI.texture = TextureDict[player.currentItem.name];

        //player.craftingUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        yield return new WaitForSeconds(0.5f);

        CanvasOFF(CraftingUI);
        StartCoroutine(HideCraftingUI());
		craftingItem = false;
        craftingCoroutine = null;
    }
    #endregion

    #region Ticker Background Control
    private void SetUpTicker()
    {
        tickerEnabledPos = tickerBackgroud.rectTransform.anchoredPosition;
        tickerDisabledPos = new Vector2(tickerEnabledPos.x, tickerEnabledPos.y - (tickerBackgroud.rectTransform.rect.height / 2));
        tickerBackgroud.rectTransform.anchoredPosition = tickerDisabledPos;
        tickerEnabled = false;
    }

    public void ShowTicker(TickerBehaviors tb, string message = null, bool isPriority = false)
    {
        if (message != null)
            currentMessage = message;

        if (isPriority)
        {
            if (!tickerEnabled)
            { //if ticker is not open and emote menu is not open
                StartCoroutine(ToggleTicker(true, tb, message));
            }
            else if (tickerEnabled && !tickerTextPlaying) //if ticker is not already playing but the emote menu is open
            {
                tickerTextPlaying = true;
                if (isPriority)
                    FadeManager.instance.ChangeMenuPanels(emoteUICG, tickerTextUICG, 0.5f);

                SetTickerMessage(message);
            }
            else //ticker already playing and player switches between cgs
            {
                if (emoteUICG.alpha == 1f)
                {
                    FadeManager.instance.ChangeMenuPanels(emoteUICG, tickerTextUICG, 0.5f);
                }
                else if (tickerTextUICG.alpha == 1f)
                {
                    FadeManager.instance.ChangeMenuPanels(tickerTextUICG, emoteUICG, 0.5f);
                }
            }
        }
        else
        {
            if (!tickerEnabled)
            { //if ticker is not open and emote menu is not open
                StartCoroutine(ToggleTicker(true, tb, message));
            }
            else
            {
                if (emoteUICG.alpha != 0f)
                {
                    FadeManager.instance.ChangeMenuPanels(emoteUICG, tickerTextUICG, 0.5f);
                }
                else if (tickerTextUICG.alpha != 0f)
                {
                    FadeManager.instance.ChangeMenuPanels(tickerTextUICG, emoteUICG, 0.5f);
                    //StopCoroutine(tickerCoroutine);
                    //tickerTween.Kill();
                }

                SetTickerMessage(currentMessage);
            }
        }
    }

    public void HideTicker()
    {
        if (tickerEnabled)
        {
            if (tickerTextPlaying && emoteUICG.alpha != 0f)
                FadeManager.instance.ChangeMenuPanels(emoteUICG, tickerTextUICG, 0.5f);
            else if (tickerTextPlaying && tickerTextUICG.alpha != 0)
                FadeManager.instance.ChangeMenuPanels(tickerTextUICG, emoteUICG, 0.5f);
            else
                StartCoroutine(ToggleTicker(false));
        }
    }

    private IEnumerator ToggleTicker(bool showTicker, TickerBehaviors tb = TickerBehaviors.None, string message = null)
    {
        if (showTicker)
        {
            bool startTicker = false;
            switch (tb)
            {
                case TickerBehaviors.Emotes:
                    emoteUICG.alpha = 1f;
                    break;

                case TickerBehaviors.TickerText:
                    startTicker = true;
                    tickerTextPlaying = true;
                    tickerTextUICG.alpha = 1f;

                    if (emoteUICG.alpha != 0f)
                        emoteUICG.alpha = 0f;
                    break;

                default:
                    Debug.Log("Ticker Behaviors defaulted, you messed up somewhere");
                    break;
            }

            tickerBackgroud.rectTransform.DOAnchorPosY(tickerEnabledPos.y, tickerLerpTime).SetEase(Ease.OutBack);

            float saveTime = Time.time;
            while (Time.time < saveTime + tickerLerpTime)
                yield return null;

            if (startTicker)
                SetTickerMessage(message);

            tickerEnabled = true;
        }
        else
        {
            tickerBackgroud.rectTransform.DOAnchorPosY(tickerDisabledPos.y, tickerLerpTime).SetEase(Ease.OutBack);

            float saveTime = Time.time;
            while (Time.time < saveTime + tickerLerpTime)
                yield return null;

            emoteUICG.alpha = 0f;
            tickerTextUICG.alpha = 0f;

            tickerEnabled = false;
        }
    }

    private void SetTickerMessage(string message)
    {
        tickerText.text = message;
        tickerCoroutine = StartCoroutine(ShowTickerMessage());
    }

    private IEnumerator ShowTickerMessage()
    {
        tickerTextPlaying = true;
        tickerTextUI.anchoredPosition = new Vector2(2000, tickerTextUI.anchoredPosition.y);

        tickerTween = tickerTextUI.DOAnchorPosX(-2500, tickerTextLerpTime).SetEase(Ease.Linear);

        float saveTime = Time.time;
        while (Time.time < saveTime + tickerTextLerpTime)
            yield return null;

        //if emote menu is not open
        if(emoteUICG.alpha != 1f)
            StartCoroutine(ToggleTicker(false));

        tickerTextPlaying = false;
        StatManager.instance.ResetTickerTimer();
    }
    #endregion

    #region Sugar Score Updates
    public void UpdateBackpackScore(int numSugar)
    {
        currentSugar = numSugar;
        BackpackScore.text = currentSugar + " / " + maxBackpackScore;

        UpdateBagUI(false);
    }

    public void UpdateMaxBackpackScore(int maxSugar)
    {
        maxBackpackScore = maxSugar;
        BackpackScore.text = currentSugar + " / " + maxBackpackScore;

        UpdateBagUI(true);
    }

    private void UpdateBagUI(bool newMax)
    {
        if (newMax)
            lineDiviser = maxBackpackScore / 5;

        int numLines = currentSugar / lineDiviser;
        int counter = 0;

        for (int x = 0; x < bagLines.Length; x++)
        {
            if (counter < numLines)
                bagLines[x].gameObject.SetActive(true);
            else
                bagLines[x].gameObject.SetActive(false);

            counter++;
        }
    }

    public void UpdateStashUI(int score)
    {
        StashScore.text = score.ToString();
    }

    public void ResetUIItemTexture()
    {
        IngameItemUI.texture = NoItemTexture;
        CraftingItemUI.texture = NoItemTexture;
    }
    #endregion

    #region Canvas Control
    private IEnumerator FadeIn(CanvasGroup c, float timeToFade, float targetAlpha = 1f)
    {
        while (c.alpha < targetAlpha)
        {
            c.alpha += Time.deltaTime / timeToFade;
            yield return null;
        }

        c.alpha = 1f;
        c.blocksRaycasts = true;
        c.interactable = true;
    }

    private IEnumerator FadeOut(CanvasGroup c, float timeToFade, float targetAlpha = 0f)
    {
        while (c.alpha > targetAlpha)
        {
            c.alpha -= Time.deltaTime / timeToFade;
            yield return null;
        }

        c.alpha = 0f;
        c.blocksRaycasts = false;
        c.interactable = false;
    }

    private void CanvasON(CanvasGroup c, bool interactable = true)
    {
        c.alpha = 1f;
        c.blocksRaycasts = interactable;
        c.interactable = interactable;
    }

    private void CanvasOFF(CanvasGroup c, bool interactable = false)
    {
        c.alpha = 0f;
        c.blocksRaycasts = interactable;
        c.interactable = interactable;
    }
    #endregion
}