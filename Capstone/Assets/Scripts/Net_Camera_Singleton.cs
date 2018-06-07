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
    

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
        LobbySingleton.instance.TransitionCam.gameObject.SetActive(false);
        LobbySingleton.instance.LobbyCam.gameObject.SetActive(false);
    }

    void PrepareCamerasForLobby()
    {
        LobbySingleton.instance.TransitionCam.gameObject.SetActive(false);
        LobbySingleton.instance.LobbyCam.gameObject.SetActive(true);
        LobbySingleton.instance.LobbyCam.enabled = true;

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

    static CinemachineVirtualCameraBase playerCam;

    [SerializeField]
    CinemachineVirtualCameraBase localCamera;


    public void SetupCamera(GameObject camTarget)
    {
        //Debug.Log("Setting camera for " + camTarget.name);
        playerCam.LookAt = camTarget.transform;
        playerCam.Follow = camTarget.transform;
    }

    public CinemachineVirtualCameraBase GetCamera()
    {
        return playerCam;
    }

}
