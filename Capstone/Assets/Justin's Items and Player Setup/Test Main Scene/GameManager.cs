using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using ckp;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    [Tooltip("In Seconds")]
    public float gameLength = 600f;
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
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }

        curPlayers = 0;
        foreach (DropoffPointsClass d in DropoffPoints)
            d.dropoffGO.SetActive(false);
    }

    public static GameManager GetGameManager()
    {
        return instance;
    }

    // Use this for initialization
    void Start()
    {
        numPlayers = GameObject.Find("LobbyManager").GetComponent<LobbyManager>()._playerNumber;
    }

    public void SetUpGame(GameObject player, net_TeamScript.Team team)
    {
        int playerNum = 0;

        Debug.Log("my team is " + team);
        //green, blue, yellow red
        DropoffPointsClass d;
        switch (team)
        {
            case net_TeamScript.Team.Zero:
                d = DropoffPoints[0];
                playerNum = 0;
                break;
            case net_TeamScript.Team.One:
                d = DropoffPoints[1];
                playerNum = 1;
                break;
            case net_TeamScript.Team.Two:
                d = DropoffPoints[2];
                playerNum = 2;
                break;
            case net_TeamScript.Team.Three:
                d = DropoffPoints[3];
                playerNum = 3;
                break;
            default:
                Debug.LogError("EXTREMELY BAD ERROR TELL CHAYANNE");
                d = DropoffPoints[0];
                playerNum = 0;
                break;
        }

        Debug.LogError("enabling " + d.dropoffGO.name);
        d.dropoffGO.SetActive(true);
        PlayerClass ply = new PlayerClass();
        ply.SetUpPlayer(playerNum, maxSugarCarry, player, d.dropoffGO, "Player " + (playerNum + 1));
        player.GetComponent<playerClassAdd>().player = ply;
        playerList.Add(ply);
        playerDropOffDict.Add(d.dropoffGO, ply);
        dropoffToPlayer.Add(ply, d);

        //temporary position and color
        //player.transform.position = ply.dropoffPoint.transform.position + new Vector3(0,20,0);

        foreach (PlayerClass p in playerList)
            p.playerGO.GetComponent<IndicatorManager>().UpdatePlayerTransforms(playerList);

        //playerClassAdd playerClass = player.GetComponent<playerClassAdd>();

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
