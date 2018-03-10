using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Networking;

public class net_PlayerCameraScript : NetworkBehaviour {

    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera lobbyCamera;
    public CinemachineVirtualCamera transitionCamera;

    public void SwitchToCameraLocal(CinemachineVirtualCamera cam)
    {
            Debug.Log("Switching!");
            GetComponent<jkuo.net_PlayerController>().virtualCam = cam;
            cam.LookAt = gameObject.transform;
            cam.Follow = gameObject.transform;
            
    }

}
