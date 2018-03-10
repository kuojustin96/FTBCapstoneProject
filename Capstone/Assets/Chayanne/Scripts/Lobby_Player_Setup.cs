using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Lobby_Player_Setup : NetworkBehaviour  {

    public Transform spawnPoint;

    public Transform playerListTransform;

    void Start()
    {

        playerListTransform = GameObject.FindGameObjectWithTag("PlayerList").transform;
        //VERY bad!
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn").transform;

        LobbyPlayerInit();
    }

    private void LobbyPlayerInit()
    {
        
        transform.parent = null;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        transform.position = spawnPoint.position;

        GetComponent<jkuo.net_PlayerController>().speed = 5;

    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if(Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Prototype.NetworkLobby.LobbyPlayer>().OnReadyClicked();
            //The worst hack i've ever done.
            transform.parent = playerListTransform;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GetComponent<Prototype.NetworkLobby.LobbyPlayer>().OnColorClicked();
        }

    }

}

    