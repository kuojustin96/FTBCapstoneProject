using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using ckp;
/*
The function OnLobbyServerSceneLoadedForPlayer will be called ONLY ON THE SERVER with the lobbyPlayer and the gamePLayer, just copy anything you
need from one to the other.As this function is called only on the server, store them in SyncVar in your gameplayer and setup color and name from that script (in the star function for exemple,
or using SyncVar hook)
    */
public class NetworkLobbyHook : LobbyHook 
{

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("Let's Go");

        int numPlayers = manager.numPlayers;

        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        net_PlayerScript player = gamePlayer.GetComponent<net_PlayerScript>();

        SyncName(lobbyPlayer, gamePlayer);


        gamePlayer.GetComponent<net_TeamScript>().SetTeam((int)lobby.GetTeamColor());

        switch (lobby.GetTeamColor())
        {
            case net_TeamScript.Team.Green:
                player.transform.position = net_GameManager.gm.greenSpawn.position;
                break;
            case net_TeamScript.Team.Purple:
                player.transform.position = net_GameManager.gm.purpleSpawn.position;
                break;
            case net_TeamScript.Team.Yellow:
                player.transform.position = net_GameManager.gm.yellowSpawn.position;
                break;
            case net_TeamScript.Team.Red:
                player.transform.position = net_GameManager.gm.redSpawn.position;
                break;

        }
    }

    private static void SyncName(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        net_PlayerScript player = gamePlayer.GetComponent<net_PlayerScript>();

        player.SetName(lobby.playerName);
    }


}
