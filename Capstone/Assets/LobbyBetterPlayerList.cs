using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
public class LobbyBetterPlayerList : NetworkBehaviour {

    public GameObject nameObject;

    public void RpcCreateName(string name)
    {

        if (name == "")
        {
            return;
        }

        GameObject obj = Instantiate(nameObject, transform);

        LobbyListName theListName = obj.GetComponent<LobbyListName>();

        Debug.Assert(theListName);

        theListName.SetName(name);
        theListName.gameObject.name = name;

        if (NetworkServer.active)
        {
            //NetworkServer.Spawn(obj);
        }

    }

    [ClientRpc]
    public void RpcRegenerateList()
    {
        NetworkLobbyPlayer[] players = LobbyManager.s_Singleton.lobbySlots;

        foreach (Transform obj in transform)
        {
            NetworkServer.Destroy(obj.gameObject);
        }

        int num = 0;

        
        //Debug.Log("there are " + LobbyManager.s_Singleton.numPlayers + " players");
        foreach (NetworkLobbyPlayer player in players)
        {

            if (player)
            {
                num++;
                Debug.Assert(player);
                Debug.Assert(player.gameObject);
                string playerName = player.gameObject.GetComponent<LobbyPlayer>().playerName;

                //Debug.Log("creating " + playerName);
                RpcCreateName(playerName);

            }

        }

    }


    [Command]
    public void CmdRegenerateList()
    {


    }

    [Command]
    public void CmdCreateName(string name)
    {


    }
}
