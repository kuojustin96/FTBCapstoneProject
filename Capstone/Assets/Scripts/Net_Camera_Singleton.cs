using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;


//sorry im bad with names this is mainly for the player camera.
public class Net_Camera_Singleton : MonoBehaviour
{

    public static Net_Camera_Singleton instance = null;

    [SerializeField]
    CinemachineVirtualCameraBase lobbyCam;

    [SerializeField]
    CinemachineVirtualCameraBase playerCam;

    [SerializeField]
    CinemachineVirtualCameraBase transitionCam;

    public static CinemachineVirtualCameraBase TransitionCam
    {
        get
        {
            return instance.transitionCam;
        }

        set
        {
            instance.transitionCam = value;
        }
    }

    public static CinemachineVirtualCameraBase PlayerCam
    {
        get
        {
            return instance.playerCam;
        }

        set
        {
            instance.playerCam = value;
        }
    }

    public static CinemachineVirtualCameraBase LobbyCam
    {
        get
        {
            return instance.lobbyCam;
        }

        set
        {
            instance.lobbyCam = value;
        }
    }

    void Start()
    {
        ValidateCameras();


        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void ValidateCameras()
    {
        Debug.Assert(LobbyCam,"Lobby Cam not set");
        Debug.Assert(PlayerCam, "Player Cam not set");
        Debug.Assert(TransitionCam, "Transition Cam not set");

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == LobbyManager.s_Singleton.lobbyScene)
        {

            Debug.Log("Prepping cameras for Lobby!!");
            PrepareCamerasForLobby();

        }
        else if (scene.name == LobbyManager.s_Singleton.playScene)
        {
            Debug.Log("Prepping cameras for Game!!");
            PrepareCamerasForGame();
        }
    }

    private void PrepareCamerasForGame()
    {
        playerCam.gameObject.SetActive(true);
        TransitionCam.gameObject.SetActive(false);
        LobbyCam.gameObject.SetActive(false);
    }

    void PrepareCamerasForLobby()
    {
        TransitionCam.gameObject.SetActive(false);
        LobbyCam.gameObject.SetActive(true);
        LobbyCam.enabled = true;

    }

    public void DebugPrepare()
    {
        Debug.LogWarning("Debug mode!");
        localCamera.Priority = 999;
    }

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            instance = this;
            playerCam = localCamera;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

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
