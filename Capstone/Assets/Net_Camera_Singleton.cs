using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Networking;

public class Net_Camera_Singleton : MonoBehaviour {

    public static Net_Camera_Singleton instance = null;

    void Awake()
    {
        Debug.Log("I HAVE WOKEN");
        if (instance == null)
        {
            instance = this;
            playerCam = localCamera;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("What the fuck are you doing????????");
        }

    }

    static CinemachineVirtualCameraBase playerCam;

    [SerializeField]
    CinemachineVirtualCameraBase localCamera;


    public void SetupCamera(GameObject camTarget)
    {
        Debug.Log("Setting camera for " + camTarget.name);
        playerCam.LookAt = camTarget.transform;
        playerCam.Follow = camTarget.transform;
    }

    public CinemachineVirtualCameraBase GetCamera()
    {
        return playerCam;
    }

}
