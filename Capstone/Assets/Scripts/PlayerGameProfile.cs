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

    public int playerHatIndex;



}
public class PlayerGameProfile : MonoBehaviour {

    public static PlayerGameProfile instance = null;

    public TMP_InputField text;

    public Color[] Colors;

    public Material[] baseColorMats;

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

        Debug.Assert(baseColorMats.Length == 4, "NOt 4 materials!");
        Debug.Assert(Colors.Length == 4, "NOt 4 Colors!");

        int index = 0;
        foreach (Material mat in baseColorMats)
        {

            Debug.Assert(mat,"YOU DIDNT FILL IT OUT MAN");
            mat.color = Colors[index];
            index++;


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

    public void UpdatePlayerOutfitSelection(int index)
    {
        PlayerPrefs.SetInt("OutfitChoice", index);
        data.playerHatIndex = index;
    }

    public int GetPlayerOutfitSelection()   
    {
        return PlayerPrefs.GetInt("OutfitChoice");
    }



	// Use this for initialization
	void Start () {

        data = new PlayerData();

        string theName = PlayerPrefs.GetString("PlayerName");
        if (theName == "")
        {
			theName = "Chad";
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
