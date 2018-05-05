using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkProfile : NetworkBehaviour {

    [SyncVar]
    string PlayerName;

    [SyncVar]
    Color PlayerColor;

    public GameObject lPlayer;

    [ClientRpc]
    public void RpcUpdateProfile(string name , Color color)
    {
        PlayerName = name;

        PlayerColor = color;
    }

    public void SetLobbyPlayer(GameObject player)
    {
        lPlayer = player;
    }
}
