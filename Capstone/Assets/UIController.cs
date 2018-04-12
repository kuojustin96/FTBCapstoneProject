﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Networking;
using DG.Tweening;

public class UIController : NetworkBehaviour {

    private PlayerClass player;
    public Texture2D cursorTexture;
    public CanvasGroup FaderPanel;

    [System.Serializable]
    public class ItemTexture
    {
        public string name;
        public Texture texture;
    }

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

    public RawImage tickerBackgroud;
    public float tickerLerpTime = 0.5f;
    private Vector2 tickerEnabledPos;
    private Vector2 tickerDisabledPos;
    public bool tickerEnabled { get; protected set; }

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
	}

    public void SetUpVariables(PlayerClass player)
    {


        origIngameUIPos = IngameItemBackgroundUI.transform.position;

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

        Cursor.lockState = CursorLockMode.Locked;
        Vector2 hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);
        Cursor.visible = true;

        FaderPanel.alpha = 1f;
        FadeManager.instance.FadeOut(FaderPanel, 1f);

        this.player = player;
        ci = player.playerGO.GetComponent<craftingInput>();
    }
	
	// Update is called once per frame
	void Update () {
        if (player == null || !isLocalPlayer)
            return;

        //Probably want to move this to OnTriggerEnter in sugar pickup
        if (player.showCraftingUI && !player.craftingUIOpen)
        {
            CanvasON(OpenCraftingUI);
        }

        //Probably want to move this to OnTriggerEnter in sugar pickup
        if (!player.showCraftingUI)
        {
            if(OpenCraftingUI.alpha == 1f)
                CanvasOFF(OpenCraftingUI);

            if (CraftingUI.alpha == 1f)
                CanvasOFF(CraftingUI);
        }


        if (Input.GetKeyDown(KeyCode.F) && player.showCraftingUI)
        {
            ToggleCraftingUI();
        }

        if (Input.GetKeyDown(KeyCode.O))
            ShowTicker();

        if (Input.GetKeyDown(KeyCode.P))
            HideTicker();
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
        //Cursor.visible = true;
    }

    private IEnumerator HideCraftingUI()
    {
        player.craftingUIOpen = false;
        CanvasOFF(CraftingUI);
        CanvasON(IngameItemBackgroundUI);

        CraftingItemFill.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
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

        IngameItemUI.texture = TextureDict[player.currentItem.name];
        CraftingItemUI.texture = TextureDict[player.currentItem.name];

        //player.craftingUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        yield return new WaitForSeconds(0.5f);

        CanvasOFF(CraftingUI);
        StartCoroutine(HideCraftingUI());
		craftingItem = false;
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

    public void ShowTicker()
    {
        if (!tickerEnabled)
            StartCoroutine(ToggleTicker(true));
    }

    public void HideTicker()
    {
        if (tickerEnabled)
            StartCoroutine(ToggleTicker(false));
    }

    private IEnumerator ToggleTicker(bool showTicker)
    {
        if (showTicker)
        {
            tickerBackgroud.rectTransform.DOAnchorPosY(tickerEnabledPos.y, tickerLerpTime).SetEase(Ease.OutBack);

            float saveTime = Time.time;
            while (Time.time < saveTime + tickerLerpTime)
                yield return null;

            tickerEnabled = true;
        }
        else
        {
            tickerBackgroud.rectTransform.DOAnchorPosY(tickerDisabledPos.y, tickerLerpTime).SetEase(Ease.OutBack);

            float saveTime = Time.time;
            while (Time.time < saveTime + tickerLerpTime)
                yield return null;

            tickerEnabled = false;
        }
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
        while(c.alpha < targetAlpha)
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