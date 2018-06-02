using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class weaponAim : NetworkBehaviour {
	public Transform mainCam;
	// Use this for initialization
	void Start () {
       // if(isLocalPlayer)
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
      //  if(isLocalPlayer)
		transform.rotation = mainCam.rotation;
	}
}
