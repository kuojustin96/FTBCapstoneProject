using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using jkuo;

public class GameOverManager : NetworkBehaviour {

    public static GameOverManager instance = null;
    [Header("End Game")]
    public GameObject[] SSModels;
    public Transform[] PersonSpots;
    public Transform endGameCamPos;
    public GameObject endGameUICanvas;
    public float waitTimeOnPerson = 3f;

    private GameManager gm;

    private Camera mainCam;

    //private List<Transform> PersonSpots = new List<Transform>();
    private List<PlayerClass> playerList = new List<PlayerClass>();
    private PlayerClass[] finalScorePlayerList;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI playerStat;
    public CanvasGroup playerScoreCG;
    public CanvasGroup playerStatCG;
    public CanvasGroup fadeBackground;

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;

        //DontDestroyOnLoad(this);
	}

    private void Start()
    {
        gm = GameManager.instance;
        DOTween.Init();
        endGameUICanvas.SetActive(false);
    }

    public void EndGame()
    {
        this.playerList = gm.playerList;
        finalScorePlayerList = new PlayerClass[playerList.Count];
        OrderPlayersBasedOnScore();
    }

    private void OrderPlayersBasedOnScore()
    {
        for (int x = 0; x < gm.playerList.Count; x++)
            finalScorePlayerList[x] = gm.playerList[x];

        //Insertion Sort
        int temp;
        PlayerClass tempPC;
        int index;
        for (int x = 1; x < finalScorePlayerList.Length; x++)
        {
            temp = finalScorePlayerList[x].currentPlayerScore;
            tempPC = finalScorePlayerList[x];
            index = x - 1;
            while (index >= 0 && finalScorePlayerList[index].currentPlayerScore > temp)
            {
                finalScorePlayerList[index + 1] = finalScorePlayerList[index];
                index--;
            }
            finalScorePlayerList[index + 1] = tempPC;
        }

        CmdEndGame();
    }

    [Command]
    private void CmdEndGame()
    {
        RpcEndGame();
    }

    [ClientRpc]
    private void RpcEndGame()
    {
        StartCoroutine(SetUpWinScene());
    }

    private IEnumerator SetUpWinScene()
    {
        gm.endGame = true;
        mainCam = Camera.main;

        FadeManager.instance.CanvasGroupOFF(playerScoreCG, false, false);
        FadeManager.instance.CanvasGroupOFF(playerStatCG, false, false);
        endGameUICanvas.SetActive(true);

        FadeManager.instance.FadeIn(fadeBackground, 1f);
        float saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        for (int x = 0; x < finalScorePlayerList.Length; x++)
        {
            GameObject g = finalScorePlayerList[x].playerGO;
            g.GetComponent<net_PlayerController>().enabled = false;
            g.GetComponent<NetworkSoundController>().CmdStopAllSFX();
            g.GetComponent<winScenePlayerController>().enabled = true;
            g.transform.position = PersonSpots[x].position;
            g.GetComponent<Rigidbody>().velocity = Vector3.zero;
            g.transform.rotation = Quaternion.LookRotation(Vector3.back);

            Net_Camera_Singleton.instance.GetCamera().enabled = false;
            Camera.main.transform.position = endGameCamPos.position;
            Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            Cursor.visible = false;

            g.GetComponent<UIController>().UICanvas.SetActive(false);
        }

        FadeManager.instance.CanvasGroupON(fadeBackground, false, false);

        StartCoroutine(WinSceneAnimation());
    }

    private IEnumerator WinSceneAnimation()
    {
        FadeManager.instance.FadeOut(fadeBackground, 1f);

        float saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        for (int x = 0; x < playerList.Count; x++)
        {
            playerScore.text = finalScorePlayerList[x].playerName + "\n" + finalScorePlayerList[x].currentPlayerScore;
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

        saveTime = Time.time;
        while (Time.time < saveTime + 4f)
            yield return null;

        FadeManager.instance.FadeIn(fadeBackground, 1f);
        saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        //if (isServer)
        //{
        //    NetworkServer.Shutdown();
        //    MasterServer.UnregisterHost();
        //    SceneManager.LoadScene(0);
        //}

        Application.Quit(); //TEMPORARY
    }
}
