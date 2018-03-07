using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    private PlayerClass player;

    public CanvasGroup OpenCraftingUI;
    public CanvasGroup CraftingUI;
    private RectTransform CraftingUIRect;
    public CanvasGroup IngameItemBackgroundUI;
    private RectTransform IngameItemRect;

    public RawImage IngameItemUI;
    private RectTransform IngameItemUIRect;
    public RawImage CraftingItemUI;
    private RectTransform CraftingItemUIRect;

    public float UIShiftTime = 1f;
    public float UIShiftSpeed = 10f;
    public float UIScaleSpeed = 0.1f;
    public float UIItemShiftSpeed = 1f;

    private Vector2 origIngameUIPos;
    private Vector2 origIngameUIScale;
    private Vector2 origIngameItemUIPos;
    private Vector2 origIngameItemUIScale;

    private craftingInput ci;
    private RectTransform lastHoveredButton;
    private Image lastHoveredImage;
    private Color saveButtonColor;

	// Use this for initialization
	void Start () {
        player = GetComponent<playerClassAdd>().player;
        ci = GetComponent<craftingInput>();

        origIngameUIPos = IngameItemBackgroundUI.transform.position;

        IngameItemRect = IngameItemBackgroundUI.GetComponent<RectTransform>();
        origIngameUIScale = IngameItemRect.sizeDelta;
        origIngameUIPos = IngameItemRect.position;

        CraftingUIRect = CraftingUI.GetComponent<RectTransform>();

        IngameItemUIRect = IngameItemUI.GetComponent<RectTransform>();
        origIngameItemUIPos = IngameItemUIRect.position;
        origIngameItemUIScale = IngameItemUIRect.sizeDelta;

        CraftingItemUIRect = CraftingItemUI.GetComponent<RectTransform>();

        CanvasOFF(OpenCraftingUI);
        CanvasOFF(CraftingUI);
        CanvasON(IngameItemBackgroundUI, false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

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
            //FToCraft.SetActive(false);
            //CraftingUI.SetActive(true);

            ToggleCraftingUI();
        }
    }

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

        Vector2 targetPos = new Vector2(Screen.width / 2, Screen.height / 2);

        float counter = 0;
        while(counter < UIShiftTime)
        {
            IngameItemBackgroundUI.transform.position = Vector2.MoveTowards(IngameItemBackgroundUI.transform.position, targetPos, Time.deltaTime * (UIShiftSpeed * 10));
            IngameItemRect.sizeDelta = Vector2.Lerp(IngameItemRect.sizeDelta, CraftingUIRect.sizeDelta, Time.deltaTime * (UIShiftSpeed * UIScaleSpeed));

            IngameItemUIRect.position = Vector2.Lerp(IngameItemUIRect.position, CraftingItemUIRect.position, Time.deltaTime * UIItemShiftSpeed);
            IngameItemUIRect.sizeDelta = Vector2.Lerp(IngameItemUIRect.sizeDelta, CraftingItemUIRect.sizeDelta, Time.deltaTime * UIItemShiftSpeed);

            counter += Time.deltaTime;
            yield return null;
        }

        IngameItemBackgroundUI.transform.position = targetPos;
        IngameItemRect.sizeDelta = CraftingUIRect.sizeDelta;
        IngameItemUIRect.position = CraftingItemUIRect.position;
        IngameItemUIRect.sizeDelta = CraftingItemUIRect.sizeDelta;
        CanvasOFF(IngameItemBackgroundUI);
        CanvasON(CraftingUI);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator HideCraftingUI()
    {
        player.craftingUIOpen = false;
        CanvasOFF(CraftingUI);
        CanvasON(IngameItemBackgroundUI);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float counter = 0;
        while (counter < UIShiftTime)
        {
            IngameItemBackgroundUI.transform.position = Vector2.MoveTowards(IngameItemBackgroundUI.transform.position, origIngameUIPos, Time.deltaTime * (UIShiftSpeed * 10));
            IngameItemRect.sizeDelta = Vector2.Lerp(IngameItemRect.sizeDelta, origIngameUIScale, Time.deltaTime * (UIShiftSpeed * UIScaleSpeed));

            IngameItemUIRect.position = Vector2.Lerp(IngameItemUIRect.position, origIngameItemUIPos, Time.deltaTime * UIItemShiftSpeed);
            IngameItemUIRect.sizeDelta = Vector2.Lerp(IngameItemUIRect.sizeDelta, origIngameItemUIScale, Time.deltaTime * UIItemShiftSpeed);

            counter += Time.deltaTime;
            yield return null;
        }

        IngameItemBackgroundUI.transform.position = origIngameUIPos;
        IngameItemRect.sizeDelta = origIngameUIScale;
        IngameItemUIRect.position = origIngameItemUIPos;
        IngameItemUIRect.sizeDelta = origIngameItemUIScale;
        CanvasON(OpenCraftingUI);
    }

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
        ci.craftAttack();
        player.craftingUIOpen = false;
        CanvasOFF(CraftingUI);
        CanvasON(OpenCraftingUI);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ClickDefenseButton()
    {
        ci.craftDefense();
        player.craftingUIOpen = false;
        CanvasOFF(CraftingUI);
        CanvasON(OpenCraftingUI);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ClickUtilityButton()
    {
        Debug.Log("Craft Utility Item (Need To Implement)");
        player.craftingUIOpen = false;
        CanvasOFF(CraftingUI);
        CanvasON(OpenCraftingUI);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


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
