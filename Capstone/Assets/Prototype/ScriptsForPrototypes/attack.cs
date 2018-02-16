using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class attack : NetworkBehaviour {
	public GameObject attackTrigger;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		if (Input.GetButtonDown ("Fire1")) {
			attackTrigger.SetActive (true);
			Invoke ("attacking", .2f);
		}
	}
	void attacking(){
		attackTrigger.SetActive (false);
	}
}
