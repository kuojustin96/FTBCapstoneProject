using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameOverManager : NetworkBehaviour {

    public static GameOverManager instance = null;
    public float waitTimeOnPerson = 3f;

    private GameManager gm;

    private Camera mainCam;

    private List<Transform> PersonSpots = new List<Transform>();
    private List<PlayerClass> playerList = new List<PlayerClass>();
    private TextMeshProUGUI playerScore;
    private TextMeshProUGUI playerStat;
    private CanvasGroup playerScoreCG;
    private CanvasGroup playerStatCG;
    private Transform playerUI;
    private CanvasGroup fadeBackground;

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
	}

    private void Start()
    {
        gm = GameManager.instance;
        DOTween.Init();
    }

    public void EndGame()
    {
        this.playerList = gm.playerList;
        //SceneManager.LoadScene("winScene");

        SceneManager.sceneLoaded += OnSceneLoaded;

        CmdSceneSwap();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Find all player UIs and store them in a list
        //Disable and UIs you don't need
        if (scene.name == "winScene" && scene.isLoaded) {
            SetUpWinScene();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }   
    }

    private void SetUpWinScene()
    {
        GameObject uiCanvas = GameObject.Find("UI Canvas");
        GameObject ps = GameObject.Find("Person Spots");

        foreach (Transform child in ps.transform)
            PersonSpots.Add(child);

        mainCam = Camera.main;
        playerUI = uiCanvas.transform.GetChild(0);
        fadeBackground = uiCanvas.transform.GetChild(1).GetComponent<CanvasGroup>();

        playerScore = playerUI.GetChild(0).GetComponent<TextMeshProUGUI>();
        playerStat = playerUI.GetChild(1).GetComponent<TextMeshProUGUI>();
        playerScoreCG = playerScore.GetComponent<CanvasGroup>();
        playerStatCG = playerStat.GetComponent<CanvasGroup>();

        FadeManager.instance.CanvasGroupOFF(playerScoreCG, false, false);
        FadeManager.instance.CanvasGroupOFF(playerStatCG, false, false);

        StartCoroutine(winSceneAnimation());
    }

    private IEnumerator winSceneAnimation()
    {
        FadeManager.instance.CanvasGroupON(fadeBackground, false, false);
        FadeManager.instance.FadeOut(fadeBackground, 1f);

        float saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        for (int x = 0; x < playerList.Count; x++)
        {
            playerScore.text = playerList[x].playerName + "\n" + playerList[x].currentPlayerScore;
            //playerStat.text Implement stat line for player from stat manager

            //Move Camera into position
            mainCam.transform.DOMoveX(PersonSpots[x].transform.position.x, 1f);
            saveTime = Time.time;
            while (Time.time < saveTime + 1f)
                yield return null;

            //Fade in stats
            FadeManager.instance.FadeIn(playerScoreCG, 0.5f);
            saveTime = Time.time;
            while (Time.time < saveTime + 0.5f)
                yield return null;

            FadeManager.instance.FadeIn(playerStatCG, 0.5f);
            saveTime = Time.time;
            while (Time.time < saveTime + 0.5f + waitTimeOnPerson) //Wait On Player
                yield return null;

            //Fade Out Stats
            FadeManager.instance.FadeOut(playerScoreCG, 0.5f);
            FadeManager.instance.FadeOut(playerStatCG, 0.5f);
            saveTime = Time.time;
            while (Time.time < saveTime + 1f) //0.5f delay before camera move
                yield return null;
        }
    }

    [Command]
    public void CmdSceneSwap()
    {
        RpcSceneSwap();
    }
    [ClientRpc]
    public void RpcSceneSwap()
    {
        SceneManager.LoadScene("winScene");
    }
}
