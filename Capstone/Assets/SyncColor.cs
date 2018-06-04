using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;

public class SyncColor : MonoBehaviour
{

    public LobbyPlayer lobbyPlayer;

    public NetworkProfile profile;

    public Renderer[] rends;

    // Use this for initialization
    void OnEnable()
    {
        bool shouldEnable = false;


        //fuck itq  
        lobbyPlayer = GetComponentInParent<LobbyPlayer>();
        if (lobbyPlayer)
        {
            shouldEnable = true;
        }

        profile = GetComponentInParent<NetworkProfile>();

        if(profile)
        {
            shouldEnable = true;
        }

        if(!lobbyPlayer && !profile)
        {
            Debug.Log("No profile!");
        }

        enabled = shouldEnable;

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Renderer r in rends)
        {
            if (lobbyPlayer)
            {
                if(r)
                    r.material.color = lobbyPlayer.playerColor;

            }
            else if(profile)
            {
                if (r)
                    r.material.color = profile.PlayerColor;

            }
        }

    }
}
