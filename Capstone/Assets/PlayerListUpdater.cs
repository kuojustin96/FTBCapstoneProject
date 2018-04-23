using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerListUpdater : NetworkBehaviour {

	void Update () {


        //
        if (isServer && isLocalPlayer)
        {
            Prototype.NetworkLobby.LobbyPlayerList._instance.theList.RpcRegenerateList();
        }
    }
}
