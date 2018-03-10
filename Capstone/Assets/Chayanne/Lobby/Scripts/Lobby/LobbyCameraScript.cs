using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LobbyCameraScript : MonoBehaviour {

    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera lobbyCamera;
    public CinemachineVirtualCamera transitionCamera;

    public CinemachineVirtualCamera GetPlayerCamera()
    {
        return playerCamera;
    }

    public CinemachineVirtualCamera GetLobbyCamera()
    {
        return lobbyCamera;
    }

    public CinemachineVirtualCamera GetTransitionCamera()
    {
        return transitionCamera;
    }
    
    public void SwitchToPlayerCamera()
    {

        playerCamera.enabled = true;
        lobbyCamera.enabled = false;
        //transitionCamera.enabled = false;
    }

    public void TransitionToDoorCamera()
    {
        playerCamera.enabled = false;
        lobbyCamera.enabled = false;
        //transitionCamera.enabled = true;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
