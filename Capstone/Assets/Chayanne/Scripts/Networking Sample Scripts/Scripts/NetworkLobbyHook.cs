using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

/*
The function OnLobbyServerSceneLoadedForPlayer will be called ONLY ON THE SERVER with the lobbyPlayer and the gamePLayer, just copy anything you
need from one to the other.As this function is called only on the server, store them in SyncVar in your gameplayer and setup color and name from that script (in the star function for exemple,
or using SyncVar hook)
    */
public class NetworkLobbyHook : LobbyHook 
{

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {

        int numPlayers = manager.numPlayers;

        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        NetMoveTest player = gamePlayer.GetComponent<NetMoveTest>();

        SyncName(lobbyPlayer, gamePlayer);

        gamePlayer.GetComponent<net_TeamScript>().SetTeam(lobby.GetTeamColorNumber());

    }

    private static void SyncName(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        NetMoveTest player = gamePlayer.GetComponent<NetMoveTest>();

        float rando =  Random.Range(0, 10);
        player.SetName(rando.ToString());
    }
}
