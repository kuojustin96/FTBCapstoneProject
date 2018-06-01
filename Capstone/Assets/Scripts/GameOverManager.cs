using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using jkuo;
using Prototype.NetworkLobby;

public class GameOverManager : NetworkBehaviour {

    public static GameOverManager instance = null;
    [Header("End Game")]
    public string[] LastPlaceAccolades;
    public string[] MiddlePlaceAccolades;
    public string[] WinnerAccolades;
    private int randAccolade;

    public GameObject[] SSModels;
    public Transform[] onePlayerSpot;
    public Transform[] twoPlayerSpots;
    public Transform[] threePlayerSpots;
    public Transform[] fourPlayerSpots;
    public Transform[] pedalstals;
    public float[] pedalstalHeights;
    private Transform[] PersonSpots;
    public Transform endGameCamPos;
    public GameObject endGameUICanvas;
    public float leadUpWaitTime = 5f;
    public float pedalstalGrowTime = 2f;

    private GameManager gm;

    private Camera mainCam;

    private PlayerClass[] finalScorePlayerList;
    public CanvasGroup[] endPlayerUIs;
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
        finalScorePlayerList = new PlayerClass[gm.playerList.Count];
        OrderPlayersBasedOnScore();
        SetPersonSpots();
        StartCoroutine(SetUpWinScene());
    }

    private void SetPersonSpots()
    {
        if (gm.playerList.Count == 1)
            PersonSpots = onePlayerSpot;
        else if (gm.playerList.Count == 2)
            PersonSpots = twoPlayerSpots;
        else if (gm.playerList.Count == 3)
            PersonSpots = threePlayerSpots;
        else
            PersonSpots = fourPlayerSpots;
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

        //CmdEndGame();
    }

    [Command]
    private void CmdEndGame()
    {
        Debug.Log("IN CMD NDSIAPNDISAPNDPISA");
        RpcEndGame();
    }

    [ClientRpc]
    private void RpcEndGame()
    {
        Debug.Log("IN RPC NIDASNDIA)SNDIANDI)A");
        StartCoroutine(SetUpWinScene());
    }

    private IEnumerator SetUpWinScene()
    {
        gm.endGame = true;
        mainCam = Camera.main;

        foreach(CanvasGroup c in endPlayerUIs)
            FadeManager.instance.CanvasGroupOFF(c, false, false);

        endGameUICanvas.SetActive(true);

        FadeManager.instance.FadeIn(fadeBackground, 1f);
        float saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        //Clients dont get placed, error with CmdStopAllSFX (maybe? double check)
        for (int x = 0; x < gm.playerList.Count; x++)
        {
            GameObject g = gm.playerList[x].playerGO;
            g.GetComponent<net_PlayerController>().enabled = false;
            //g.GetComponent<NetworkSoundController>().CmdStopAllSFX();
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

        FadeManager.instance.FadeOut(fadeBackground, 1f);
        saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        FadeManager.instance.CanvasGroupOFF(fadeBackground, false, false);
        StartCoroutine(WinSceneAnimation());
    }

    private IEnumerator WinSceneAnimation()
    {
        int middlePlaceTemp = -1;
        FadeManager.instance.FadeOut(fadeBackground, 1f);

        float saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        for (int x = 0; x < gm.playerList.Count; x++)
        {
            endPlayerUIs[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = finalScorePlayerList[x].playerName + "\n" + finalScorePlayerList[x].currentPlayerScore;
            string title = "";
            if (x == gm.playerList.Count - 1)
            {
                if (isServer)
                {
                    randAccolade = Random.Range(0, WinnerAccolades.Length);
                    CmdSyncRand(randAccolade);
                }
                title = WinnerAccolades[randAccolade];
            }
            else if (x == 0)
            {
                if (isServer)
                {
                    randAccolade = Random.Range(0, LastPlaceAccolades.Length);
                    CmdSyncRand(randAccolade);
                }
                title = LastPlaceAccolades[randAccolade];
            }
            else
            {
                bool picking = true;
                while (picking)
                {
                    if (isServer)
                    {
                        randAccolade = Random.Range(0, MiddlePlaceAccolades.Length);
                        CmdSyncRand(randAccolade);
                    }
                    if (randAccolade != middlePlaceTemp)
                    {
                        middlePlaceTemp = randAccolade;
                        title = MiddlePlaceAccolades[randAccolade];
                        picking = false;
                    }
                }
            }

            endPlayerUIs[x].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        }
        
        //drumroll music
        saveTime = Time.time; //Time from the start of the animation, until winner is revealed
        while (Time.time < saveTime + leadUpWaitTime)
            yield return null;

        //winning music
        for(int x = 0; x < gm.playerList.Count; x++)
        {
            pedalstals[x].transform.position = new Vector3(finalScorePlayerList[x].playerGO.transform.position.x, pedalstals[x].transform.position.y, finalScorePlayerList[x].playerGO.transform.position.z);
            pedalstals[x].DOScaleY(pedalstalHeights[x], pedalstalGrowTime);
            gm.playerList[x].playerGO.transform.DOMoveY(gm.playerList[x].playerGO.transform.position.y + (pedalstalHeights[x] / 2), pedalstalGrowTime);
        }

        saveTime = Time.time; //Time it takes the pedastals to grow
        while (Time.time < saveTime + pedalstalGrowTime)
            yield return null;

        for(int x = 0; x < gm.playerList.Count; x++)
        {
            FadeManager.instance.FadeIn(endPlayerUIs[x], 1f, 1f);
            endPlayerUIs[x].transform.position = new Vector3(pedalstals[x].transform.position.x, endPlayerUIs[x].transform.position.y, endPlayerUIs[x].transform.position.z);
        }


        saveTime = Time.time; //Time from end of animation until the game auto sends you to the menu
        while (Time.time < saveTime + 15f)
            yield return null;

        FadeManager.instance.FadeIn(fadeBackground, 1f);
        saveTime = Time.time;
        while (Time.time < saveTime + 1f)
            yield return null;

        if(LobbyManager.IsLocalPlayerHost())
        {
            LobbyManager.s_Singleton.StopHost();
        }
        else
        {
            LobbyManager.s_Singleton.StopClient();
        }
    }

    [Command]
    private void CmdSyncRand(int rand)
    {
        RpcSyncRand(rand);
    }

    [ClientRpc]
    private void RpcSyncRand(int rand)
    {
        randAccolade = rand;
    }
}
