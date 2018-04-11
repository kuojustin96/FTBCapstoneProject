using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Lobby_Player_Setup : NetworkBehaviour  {

    public Transform spawnPoint;

    public Transform playerListTransform;

    public float fadeTime = 1.0f;

    bool isReady = false;

    void Start()
    {
        transform.parent = null;
        transform.rotation = Quaternion.Euler(new Vector3(0, spawnPoint.rotation.eulerAngles.y, 0));
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        transform.position = spawnPoint.position;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if(! isReady && Input.GetKeyDown(KeyCode.G))
        {
            isReady = true;

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

    