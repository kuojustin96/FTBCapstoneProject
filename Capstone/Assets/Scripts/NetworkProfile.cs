using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;
public class NetworkProfile : NetworkBehaviour {


    [SyncVar]
    [SerializeField]
    string PlayerName;

    [SyncVar]
    [SerializeField]
    Color PlayerColor;

    [SyncVar]
    public int myNum;

    [SyncVar]
    public int outfitChoice;

    playerClassAdd pca;
    NetworkOutfitScript nos;


    void Start()
    {
        nos = GetComponent<NetworkOutfitScript>();
        pca = GetComponent<playerClassAdd>();
        Debug.Log("my name is " + PlayerName);
        pca.player.playerName = PlayerName;
        pca.SetMaterialColor(PlayerColor, myNum);
        GameManager.instance.SetUpBaseName(pca.player);
    }

    public void UpdateProfile(string name, Color color, int num, int outfitNum)
    {
        PlayerName = name;
        PlayerColor = color;
        myNum = num;
        outfitChoice = outfitNum;

        nos = GetComponent<NetworkOutfitScript>();

        nos.currentHat = outfitChoice;
        nos.theColor = color;

        Debug.Log("okay your outfit is " + outfitChoice);

    }

    public void CopyProfile(LobbyPlayer player)
    {
        nos = GetComponent<NetworkOutfitScript>();

        PlayerName = player.playerName;
        PlayerColor = player.playerColor;
        myNum = player.playerNum;
        outfitChoice = player.outfitNum;

        nos.theColor = PlayerColor;
        nos.currentHat = outfitChoice;
        nos.ChangeHat(outfitChoice);
    }

    //public void RpcUpdateProfile(string name, Color color,GameObject player)
    //{
    //    Debug.Log("meme");
    //    PlayerName = name; 
    //    PlayerColor = color;

    //    gPlayer = player;

    //    Debug.Assert(gPlayer);
    //    Debug.Assert(gPlayer.GetComponent<playerClassAdd>());
    //    Debug.Assert(gPlayer.GetComponent<playerClassAdd>().player != null);

    //    Debug.Log("Updating profile!" + " Name: " + name);
    //    gPlayer.GetComponent<playerClassAdd>().player.playerName = name;
    //    gPlayer.GetComponent<playerClassAdd>().SetMaterialColor(color, myNum);

    //}

    //public void SetGamePlayer(GameObject player)
    //{
    //    Debug.Log("Game player set!");
    //    gPlayer = player;
    //}

}
