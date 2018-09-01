using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;  
public class Lobby_Player_Movement : ChadController
{
    public override float GetGlidingFatigue()
    {
        return 0.0f;
    }

    public override bool IsMenuOpen()
    {
        return false;
    }

    public override void UpdateSliders()
    {
    }

    protected override void LocalCameraCheck()
    {
    }

    protected override void Update()
    {
        base.Update();

        if (!isLocalPlayer)
            return;
        if(SceneManager.GetActiveScene().name == LobbyManager.s_Singleton.lobbyScene)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}