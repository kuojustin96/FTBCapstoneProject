using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using Prototype.NetworkLobby;
public class NetworkEmoteScript : NetworkBehaviour {


    [Header("Autofill from manager")]
    public RawImage tickerBackgroud;
    public GameObject emoteUI;
    public CanvasGroup emoteUICG;

    [Header("Other")]
    public float tickerLerpTime = 10f;
    public float tickerTextLerpTime = 10f;
    public ParticleSystem[] Emotes;
    public bool emoteMenuOpen = false;


    private bool tickerEnabled = false;
    private bool playingEmote = false;
    private Vector2 tickerEnabledPos;
    private Vector2 tickerDisabledPos;
    private UIController uic;
    private NetworkSoundController nsc;
    private float defaultMaxDist;

    private void Start()
    {
        if (isLocalPlayer)
        {
            LobbyManager manager = LobbyManager.s_Singleton;
            tickerBackgroud = manager.tickerBackgroud;
            emoteUI = manager.emoteUI;
            emoteUICG = manager.emoteUICG;

            tickerBackgroud.gameObject.SetActive(true);

            uic = GetComponent<UIController>();
            nsc = GetComponent<NetworkSoundController>();
            defaultMaxDist = SoundEffectManager.instance.defaultMaxDist;
            tickerEnabledPos = tickerBackgroud.rectTransform.anchoredPosition;
            tickerDisabledPos = new Vector2(tickerEnabledPos.x, tickerEnabledPos.y - (tickerBackgroud.rectTransform.rect.height / 2));
            tickerBackgroud.rectTransform.anchoredPosition = tickerDisabledPos;
            tickerEnabled = false;
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            UseEmotes();
        }
    }

    #region Play Emotes
    private void UseEmotes()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            emoteMenuOpen = !emoteMenuOpen;
            StartCoroutine(ShowText(emoteMenuOpen));
        }

        if (emoteMenuOpen && !playingEmote)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                nsc.CmdPlaySFX("Angry", gameObject, 1f, defaultMaxDist, false, false);
                CmdEmote(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                nsc.CmdPlaySFX("Taunt", gameObject, 1f, defaultMaxDist, false, false);
                CmdEmote(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                nsc.CmdPlaySFX("Cheer", gameObject, 1f, defaultMaxDist, false, false);
                CmdEmote(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                nsc.CmdPlaySFX("Cheer", gameObject, 1f, defaultMaxDist, false, false);
                CmdEmote(3);
            }
        }

    }

    [ClientRpc]
    private void RpcEmote(int emoteNum)
    {
        Emotes[emoteNum].Play();
        emoteMenuOpen = false;
        playingEmote = true;

        if (!isLocalPlayer)
        {
            // code run on other players
            Vector3 temp = Emotes[emoteNum].transform.localScale;
            temp.x = -1;
            Emotes[emoteNum].transform.localScale = temp;
        }
        else
        {
            StartCoroutine(c_EmoteCooldown(emoteNum));
        }
    }

    [Command]
    private void CmdEmote(int emoteNum)
    {
        RpcEmote(emoteNum);
    }

    
    private IEnumerator ShowText(bool showTicker)
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

    private IEnumerator c_EmoteCooldown(int emoteNum)
    {
        emoteMenuOpen = false;
        StartCoroutine(ShowText(false));

        float saveTime = Time.time;
        float psDuration = Emotes[emoteNum].main.duration;
        while (Time.time < saveTime + psDuration)
            yield return null;

        playingEmote = false;
    }
    #endregion
}
