using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKitScript : MonoBehaviour {

    public Lobby_Player_Movement player;
    public Net_Camera_Singleton cameraManager;

    Lobby_Player_Movement playerInstance;
    void Awake () {

        playerInstance = Instantiate(player, transform);
        Instantiate(cameraManager, transform);
    }
	
	void Update () {

        playerInstance.offlineTesting = true;
        playerInstance.GetComponent<PlayerAttachCamera>().offlineTesting = true;
        playerInstance.gameObject.SetActive(true);
        playerInstance.netAnim.gameObject.SetActive(true);


    }


}
