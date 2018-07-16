using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;

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
}