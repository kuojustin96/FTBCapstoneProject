using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkProfile : NetworkBehaviour {


    [SyncVar]
    [SerializeField]
    string PlayerName;

    [SyncVar]
    [SerializeField]
    Color PlayerColor;

    [SyncVar]
    int myNum;

    [SyncVar]
    int outfitChoice;

    playerClassAdd pca;
    NetworkOutfitScript nos;


    void Start()
    {
        nos = GetComponent<NetworkOutfitScript>();
        pca = GetComponent<playerClassAdd>();
        Debug.Log("my name is " + PlayerName);
        pca.player.playerName = PlayerName;
        pca.SetMaterialColor(PlayerColor, myNum);
        nos.ChangeHat(outfitChoice, PlayerColor);

        GameManager.instance.SetUpBaseName(pca.player);
    }

    public void UpdateProfile(string name, Color color, int num, int outfitNum)
    {
        PlayerName = name;
        PlayerColor = color;
        myNum = num;
        outfitChoice = outfitNum;
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
