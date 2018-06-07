using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
public class LobbyBetterPlayerList : MonoBehaviour {

    public GameObject nameObject;


    private void Start()
    {

        LobbyManager.s_Singleton.playerList = this;

    }

    public void CreateName(string name)
    {
    
        if (name == "")
        {
            Debug.Log("BAD");
            return;
        }

        GameObject obj = Instantiate(nameObject, transform);

        LobbyListName theListName = obj.GetComponent<LobbyListName>();

        Debug.Assert(theListName);

        theListName.SetName(name);
        theListName.gameObject.name = name;


    }

    public void RemoveName(string name)
    {
        //Debug.Log("removing" + name);
        foreach(Transform t in transform)
        {
            if(t.name == name)
            {
                Debug.Log(name + "Left the game!~");
                Debug.Log("Found it!");
                Destroy(t.gameObject);
            }
        }
    }

    //public void RpcRegenerateList()
    //{
    //    NetworkLobbyPlayer[] players = LobbyManager.s_Singleton.lobbySlots;

    //    foreach (Transform obj in transform)
    //    {
    //        NetworkServer.Destroy(obj.gameObject);
    //    }

    //    int num = 0;

        
    //    //Debug.Log("there are " + LobbyManager.s_Singleton.numPlayers + " players");
    //    foreach (NetworkLobbyPlayer player in players)
    //    {

    //        if (player)
    //        {
    //            num++;
    //            Debug.Assert(player);
    //            Debug.Assert(player.gameObject);
    //            string playerName = player.gameObject.GetComponent<LobbyPlayer>().playerName;

    //            //Debug.Log("creating " + playerName);
    //            CreateName(playerName);

    //        }

    //    }

    //}


    //[Command]
    //public void CmdRegenerateList()
    //{


    //}

    //[Command]
    //public void CmdCreateName(string name)
    //{
    //    CreateName(name);

    //}
}
