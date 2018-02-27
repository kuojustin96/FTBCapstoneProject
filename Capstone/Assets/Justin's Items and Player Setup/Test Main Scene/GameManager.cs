using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using ckp;

public class GameManager : NetworkBehaviour {

    public static GameManager instance = null;

    public float sugarPickUpSpeed = 0.2f;
    public float dropoffDelay = 0.5f;
    public int maxSugarCarry = 10;
	public static int curPlayers;

    public GameObject playerPrefab;
    public Material[] playerMats;
    //public List<GameObject> DropoffPoints = new List<GameObject>();

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
        }

        foreach(DropoffPointsClass d in DropoffPoints)
        {
            d.dropoffGO.SetActive(false);
        }

		curPlayers = 0;
    }

	// Use this for initialization
	void Start () {
        numPlayers = GameObject.Find("LobbyManager").GetComponent<LobbyManager>()._playerNumber;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUpGame(GameObject player, net_TeamScript.Team team)
    {
		// Not working color change 
//        foreach(DropoffPointsClass d in DropoffPoints)
//        {
//            if(team == d.teamColor)
//            {
//                int x = curPlayers + 1;
//                d.dropoffGO.SetActive(true);
//				Debug.Log (d.dropoffGO);
//                PlayerClass ply = new PlayerClass();
//                ply.SetUpPlayer(x, maxSugarCarry, player, d.dropoffGO, "Player " + x);
//                player.GetComponent<playerClassAdd>().player = ply;
//                playerList.Add(ply);
//                playerDropOffDict.Add(d.dropoffGO, ply);
//
//                //      ScoreController.instance.SetUpScoreController(x);
//                curPlayers++;
//                Debug.Log("DSADSA");
//            }
//        }
		int x = curPlayers ;
//		                d.dropoffGO.SetActive(true);
//						Debug.Log (d.dropoffGO);
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
		                //      ScoreController.instance.SetUpScoreController(x);
		                curPlayers++;
		                Debug.Log("DSADSA");
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
