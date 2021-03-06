﻿using UnityEngine;
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
    //TODO: sync name and physicsl material color

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("Let's Go");



        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        jkuo.net_PlayerController player = gamePlayer.GetComponent<jkuo.net_PlayerController>();

        int playerNum = lobby.playerNum;
        SyncVariables(lobbyPlayer, gamePlayer, playerNum);

        net_TeamScript team = gamePlayer.GetComponent<net_TeamScript>();

        team.SetTeam((int)lobby.GetTeamColor());
        team.color = lobby.playerColor;



        switch (lobby.GetTeamColor())
        {
            case net_TeamScript.Team.Zero:
                Vector3 temp = net_GameManager.instance.ZeroSpawn.position;
                player.transform.position = new Vector3(temp.x, temp.y + 10, temp.z);
            //    GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Green);
                break;
            case net_TeamScript.Team.One:
                Vector3 temp2 = net_GameManager.instance.OneSpawn.position;
                player.transform.position = new Vector3(temp2.x, temp2.y + 10, temp2.z);
             //   GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Purple);
                break;
            case net_TeamScript.Team.Two:
                Vector3 temp3 = net_GameManager.instance.TwoSpawn.position;
                player.transform.position = new Vector3(temp3.x, temp3.y + 10, temp3.z);
            //    GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Yellow);
                break;
            case net_TeamScript.Team.Three:
                Vector3 temp4 = net_GameManager.instance.ThreeSpawn.position;
                player.transform.position = new Vector3(temp4.x, temp4.y + 10, temp4.z);
              //  GameManager.instance.SetUpGame(gamePlayer, net_TeamScript.Team.Red);
                break;

        }
    }


    private static void SyncVariables(GameObject lobbyPlayer, GameObject gamePlayer, int playerNum)
    {

        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();


        //string name = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
        //Color color = lobbyPlayer.GetComponent<LobbyPlayer>().playerColor;
        //int outfit  = lobbyPlayer.GetComponent<LobbyPlayer>().outfitNum;
        //profile.UpdateProfile(name, color, playerNum,outfit);

        gamePlayer.GetComponent<NetworkProfile>().CopyProfile(lp);



        Debug.Log("Synced my names!");
        Debug.Assert(gamePlayer.GetComponent<NetworkProfile>(),"Rip me");
        //gamePlayer.GetComponent<NetworkProfile>().SetLobbyPlayer(lobbyPlayer);
        //string name = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
        //Color color = lobbyPlayer.GetComponent<LobbyPlayer>().playerColor;
        //gamePlayer.GetComponent<NetworkProfile>().RpcUpdateProfile(name,color);
        Debug.Log("Joined the game");
    }


}
