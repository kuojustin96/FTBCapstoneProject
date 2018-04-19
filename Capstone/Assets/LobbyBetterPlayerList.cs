using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
public class LobbyBetterPlayerList : NetworkBehaviour {

    public GameObject nameObject;

    [ClientRpc]
    public void RpcCreateName(string name)
    {

        GameObject obj = Instantiate(nameObject, transform);

        LobbyListName theListName = obj.GetComponent<LobbyListName>();

        Debug.Assert(theListName);

        theListName.SetName(name);
        theListName.gameObject.name = name;

        NetworkServer.Spawn(obj);

    }
    [ClientRpc]
    public void RpcRegenerateList()
    {
        Debug.Log("wow");
        NetworkLobbyPlayer[] players = LobbyManager.s_Singleton.lobbySlots;

        foreach (Transform obj in transform)
        {
            if (obj != transform.gameObject && obj.GetComponent<NetworkIdentity>())
            {   
                NetworkServer.Destroy(obj.gameObject);
            }
        }

        foreach (NetworkLobbyPlayer player in players)
        {
            if (player)
            {
                Debug.Assert(player);
                Debug.Assert(player.gameObject);
                string playerName = player.gameObject.GetComponent<LobbyPlayer>().playerName;

                RpcCreateName(playerName);
            }

        }

    }

}
