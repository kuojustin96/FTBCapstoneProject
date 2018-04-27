using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prototype.NetworkLobby;
using ckp;

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

    public GameObject playerPrefab;
    public Material[] playerMats;

    [System.Serializable]
    public class DropoffPointsClass
    {
        public string name;
        public GameObject dropoffGO;
        public net_TeamScript.Team teamColor;
    }

    public DropoffPointsClass[] DropoffPoints;
    public List<PlayerClass> playerList = new List<PlayerClass>();

    private Dictionary<string, GameObject> colorDropOffDict = new Dictionary<string, GameObject>();
    public Dictionary<GameObject, PlayerClass> playerDropOffDict = new Dictionary<GameObject, PlayerClass>();

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
		ply.SetUpPlayer(x, maxSugarCarry, player, d.dropoffGO, "Player " + x);
        player.GetComponent<playerClassAdd>().player = ply;
        playerList.Add(ply);
        playerDropOffDict.Add(d.dropoffGO, ply);

		//temporary position and color
		player.transform.position = ply.dropoffPoint.transform.position + new Vector3(0,20,0);
		player.GetComponent<playerClassAdd> ().Hood.GetComponent<Renderer> ().material = playerMats [x];
		player.GetComponent<playerClassAdd> ().Cloak.GetComponent<Renderer> ().material = playerMats [x];
		curPlayers++;
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
