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
    //[Command]
    //public void CmdUpdateProfile(string name, Color color)
    //{
    //    RpcUpdateProfile(name,color,PlayerGameProfile.instance.GetGamePlayer());
    //}

    //[ClientRpc]
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

    void Start()
    {

        Debug.Log("my name is " + PlayerName);
        GetComponent<playerClassAdd>().player.playerName = PlayerName;
        GetComponent<playerClassAdd>().SetMaterialColor(PlayerColor, myNum);

    }

    public void UpdateProfile(string name, Color color, int num)
    {
        PlayerName = name;
        PlayerColor = color;
        myNum = num;
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
