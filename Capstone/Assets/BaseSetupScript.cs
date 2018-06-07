using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ckp;
using Prototype.NetworkLobby;
public class BaseSetupScript : NetworkBehaviour {


    void Start()
    {
        net_GameManager.instance.SetupPlayerBase(gameObject);
    }

    void OnDestroy()
    {
        LobbyManager.s_Singleton.UnregisterPlayer(gameObject);)
    }
}
