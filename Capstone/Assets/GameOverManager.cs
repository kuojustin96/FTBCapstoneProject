using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameOverManager : NetworkBehaviour {

    public static GameOverManager instance = null;

    private GameManager gm;

    GameObject[] RootGameobjects;

    private List<PlayerClass> playerList = new List<PlayerClass>();
    private List<Transform> playerCanvases = new List<Transform>();

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
	}

    private void Start()
    {
        gm = GameManager.instance;
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
        //RootGameobjects = SceneManager.GetSceneByName("winScene").GetRootGameObjects();
        //GameObject uiCanvas = null;

        //foreach(GameObject g in RootGameobjects)
        //{
        //    if (g.name.Equals("UI Canvas"))
        //    {
        //        uiCanvas = g;
        //        break;
        //    }
        //}

        GameObject uiCanvas = GameObject.Find("UI Canvas");

        foreach (Transform child in uiCanvas.transform)
        {
            if (child.name.Contains("Player"))
            {
                playerCanvases.Add(child);
            }
        }

        int playerCount = playerList.Count;

        //Remake to use playerCanvases
        for (int x = 0; x < playerList.Count; x++)
        {
            Transform playerScore = playerCanvases[x].transform.GetChild(0);
            playerScore.GetComponent<TextMeshProUGUI>().text = "Player " + (x + 1) + " Score:\n" + playerList[x].currentPlayerScore;
        }

        for (int x = (4- (4 - playerCount)); x < playerCanvases.Count; x++)
        {
            playerCanvases[x].gameObject.SetActive(false);
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
