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

    //The problem is that the lobby hook, on builds, executes 
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("Let's Go");

        int numPlayers = manager.numPlayers;

        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        jkuo.net_PlayerController player = gamePlayer.GetComponent<jkuo.net_PlayerController>();

        SyncName(lobbyPlayer, gamePlayer);


        gamePlayer.GetComponent<net_TeamScript>().SetTeam((int)lobby.GetTeamColor());

        switch (lobby.GetTeamColor())
        {
            case net_TeamScript.Team.Green:
                Vector3 temp = net_GameManager.instance.greenSpawn.position;
                player.transform.position = new Vector3(temp.x, temp.y + 10, temp.z);
            //    GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Green);
                break;
            case net_TeamScript.Team.Purple:
                Vector3 temp2 = net_GameManager.instance.purpleSpawn.position;
                player.transform.position = new Vector3(temp2.x, temp2.y + 10, temp2.z);
             //   GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Purple);
                break;
            case net_TeamScript.Team.Yellow:
                Vector3 temp3 = net_GameManager.instance.yellowSpawn.position;
                player.transform.position = new Vector3(temp3.x, temp3.y + 10, temp3.z);
            //    GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Yellow);
                break;
            case net_TeamScript.Team.Red:
                Vector3 temp4 = net_GameManager.instance.redSpawn.position;
                player.transform.position = new Vector3(temp4.x, temp4.y + 10, temp4.z);
              //  GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Red);
                break;

        }
    }

    private static void SyncName(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log(lobbyPlayer.GetComponent<LobbyPlayer>().playerName + "Joined the game");
    }


}
