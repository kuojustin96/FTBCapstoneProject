using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CameraChec : NetworkBehaviour {
    public GameObject Camera;
	// Use this for initialization
	void Start () {

		if (!GetComponent<NetMoveTest> ().isLocalPlayer) {
            Camera.GetComponent<Camera> ().enabled = false;
            Camera.GetComponent<AudioListener> ().enabled = false;
			}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
