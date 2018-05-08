using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Prototype.NetworkLobby;

[System.Serializable]
public struct PlayerData
{
    public string name;
    public Color color;
    public LobbyPlayer localLobbyPlayer;
    public GameObject localGamePlayer;
}
public class PlayerGameProfile : MonoBehaviour {

    public static PlayerGameProfile instance = null;

    public TMP_InputField text;


    [SerializeField]
    PlayerData data;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }


        DontDestroyOnLoad(gameObject);
    }


    public void SetLobbyPlayer(Prototype.NetworkLobby.LobbyPlayer player)
    {
        data.localLobbyPlayer = player;
    }

    public void SetGamePlayer(GameObject player)
    {
        data.localGamePlayer = player;
    }

    public GameObject GetGamePlayer()
    {
        return data.localGamePlayer;
    }

    public LobbyPlayer GetLocalLobbyPlayer()
    {
        return data.localLobbyPlayer;
    }

    public PlayerData GetPlayerData()
    {
        return data;
    }

    public void UpdatePlayerName()
    {
        //Debug.Log("Name is " + text.text);
        data.name = text.text;

        PlayerPrefs.SetString("PlayerName", data.name);    
        
    }



	// Use this for initialization
	void Start () {

        data = new PlayerData();

        string theName = PlayerPrefs.GetString("PlayerName");
        if (PlayerPrefs.GetString("PlayerName") == "")
        {
            data.name = "Chad";
        }
        else
        {
            data.name = theName;
        }
        //updates the name field;
        text.text = theName;

        data.name = theName;

    }

}
