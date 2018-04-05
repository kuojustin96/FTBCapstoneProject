using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Networking;

public class net_PlayerCameraScript : NetworkBehaviour {

    public CinemachineVirtualCameraBase playerCamera;
    public CinemachineVirtualCameraBase lobbyCamera;
    public CinemachineVirtualCameraBase transitionCamera;

    public void SwitchToCameraLocal(CinemachineVirtualCameraBase cam)
    {
            Debug.Log("Switching!");
            GetComponent<Lobby_Player_Movement>().mainCam = cam;
            cam.LookAt = gameObject.transform;
            cam.Follow = gameObject.transform;
            
    }

}
