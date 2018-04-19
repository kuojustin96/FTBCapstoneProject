using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class Lobby_Player_Setup : NetworkBehaviour  {


    public float fadeTime = 1.0f;

    bool isReady = false;

    bool detatched = false;

    bool doUpdate = false;

    void Start()
    {


    }

    void Update()
    {




        if (!detatched)
        {
            detatched = true;
            //transform.parent = null;

            //Transform spawn = Prototype.NetworkLobby.LobbyManager.s_Singleton.lobbySpawn.transform;

            //transform.rotation = Quaternion.Euler(new Vector3(0, spawn.eulerAngles.y, 0));
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //transform.position = spawn.position;
        }

        //Detatches from the lobby prefab.
        //Never gonna change so why not. 


        if (!isLocalPlayer)
            return;

        if(! isReady && Input.GetKeyDown(KeyCode.G))
        {
            isReady = true;

            GetComponent<Prototype.NetworkLobby.LobbyPlayer>().OnReadyClicked();
            //The worst hack i've ever done.
            transform.parent = LobbyManager.s_Singleton.playerListTransform;

        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GetComponent<Prototype.NetworkLobby.LobbyPlayer>().OnColorClicked();
        }



    }





}

    