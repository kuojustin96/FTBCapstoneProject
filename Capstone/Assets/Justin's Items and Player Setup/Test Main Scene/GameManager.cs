using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public float sugarPickUpSpeed = 0.2f;
    public float dropoffDelay = 0.5f;
    public int maxSugarCarry = 10;
	public static int curPlayers;

    public GameObject playerPrefab;
    public Material[] playerMats;
    public List<GameObject> DropoffPoints = new List<GameObject>();
    public List<PlayerClass> playerList = new List<PlayerClass>();

    public Dictionary<GameObject, PlayerClass> playerDropOffDict = new Dictionary<GameObject, PlayerClass>();

    [HideInInspector]
    public int numPlayers;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        foreach(GameObject g in DropoffPoints)
        {
            g.SetActive(false);
        }

		curPlayers = 1;
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUpGame(GameObject player)
    {
		int x = curPlayers;
        DropoffPoints[x].SetActive(true);
//            GameObject player = Instantiate(playerPrefab, new Vector3(DropoffPoints[x].transform.position.x, 
//                                                            DropoffPoints[x].transform.position.y + 1,
//                                                            DropoffPoints[x].transform.position.z), Quaternion.identity);
//            player.GetComponent<MeshRenderer>().material = playerMats[x];
        PlayerClass ply = new PlayerClass();
        ply.SetUpPlayer(x, maxSugarCarry, player, DropoffPoints[x], "Player " + x);
		player.GetComponent<playerClassAdd>().player = ply;
        playerList.Add(ply);
        playerDropOffDict.Add(DropoffPoints[x], ply);

//            ScoreController.instance.SetUpScoreController(x);
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
