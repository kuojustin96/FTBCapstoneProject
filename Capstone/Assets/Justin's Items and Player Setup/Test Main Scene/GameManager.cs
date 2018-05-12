using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prototype.NetworkLobby;
using ckp;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public int numSugarToWin = 50;
    public float sugarPickupTime = 0.5f;
    public float dropoffDelay = 0.5f;
    public int maxSugarCarry = 10;
    [Tooltip("Amount of speed gained/lost when dropping/picking up sugar")]
    public float speedPerSugar = 0.5f;
    public float minSpeed = 20f;
	public static int curPlayers;

    public bool endGame = false;

    public GameObject playerPrefab;
    public Material[] playerMats;
    public string[] fortNames;

    [System.Serializable]
    public class DropoffPointsClass
    {
        public string name;
        public GameObject dropoffGO;
        public TextMeshProUGUI[] FortNamesTMP;
        public net_TeamScript.Team teamColor;

        public string PickRandomFortName()
        {
            GameManager gm = GameManager.instance;
            int rand = Random.Range(0, gm.fortNames.Length);
            return gm.fortNames[rand];
        }
    }

    public DropoffPointsClass[] DropoffPoints;
    public List<PlayerClass> playerList = new List<PlayerClass>();

    private Dictionary<string, GameObject> colorDropOffDict = new Dictionary<string, GameObject>();
    public Dictionary<GameObject, PlayerClass> playerDropOffDict = new Dictionary<GameObject, PlayerClass>();
    public Dictionary<PlayerClass, DropoffPointsClass> dropoffToPlayer = new Dictionary<PlayerClass, DropoffPointsClass>();

    [HideInInspector]
    public int numPlayers;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

		curPlayers = 0;
        foreach (DropoffPointsClass d in DropoffPoints)
            d.dropoffGO.SetActive(false);
    }

    public static GameManager GetGameManager()
    {
        return instance;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
        //{
        //    GameObject[] temp = GameObject.FindGameObjectsWithTag("Dropoff Point");
        //    GameObject red = GameObject.Find("Red");

        //    Debug.Log("RED" + red);

        //    Debug.Log("YOOOOOOOOOOOO " + temp.Length);

        //    for (int x = 0; x < 4; x++)
        //    {
        //        DropoffPointsClass dpc = new DropoffPointsClass();
        //        dpc.name = temp[x].name;
        //        dpc.dropoffGO = temp[x];
        //        dpc.teamColor = (net_TeamScript.Team)x;

        //        temp[x].SetActive(false);
        //    }
        //}
    }

    // Use this for initialization
    void Start () {
        numPlayers = GameObject.Find("LobbyManager").GetComponent<LobbyManager>()._playerNumber;
	}

	public void SetUpGame(GameObject player, net_TeamScript.Team team)
    {
		int x = curPlayers;
		DropoffPointsClass d = DropoffPoints[x];
		d.dropoffGO.SetActive (true);
        PlayerClass ply = new PlayerClass();
		ply.SetUpPlayer(x, maxSugarCarry, player, d.dropoffGO, "Player " + (x + 1));
        player.GetComponent<playerClassAdd>().player = ply;
        playerList.Add(ply);
        playerDropOffDict.Add(d.dropoffGO, ply);
        dropoffToPlayer.Add(ply, d);

		//temporary position and color
		player.transform.position = ply.dropoffPoint.transform.position + new Vector3(0,20,0);

        playerClassAdd playerClass = player.GetComponent<playerClassAdd>();

		curPlayers++;
    }

    public void SetUpBaseName(PlayerClass ply)
    {
        DropoffPointsClass d = dropoffToPlayer[ply];
        for (int y = 0; y < d.FortNamesTMP.Length; y++)
        {
            d.FortNamesTMP[y].text = d.PickRandomFortName() + " " + ply.playerName;
        }
    }

    public PlayerClass GetPlayer(int playerNum)
    {
        return playerList[playerNum];
    }

    public PlayerClass GetPlayerFromDropoff(GameObject dropoffPoint)
    {
        return playerDropOffDict[dropoffPoint];
    }
}
