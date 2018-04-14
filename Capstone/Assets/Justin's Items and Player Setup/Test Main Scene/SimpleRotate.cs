using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleRotate : NetworkBehaviour {

    private float origYPos;

	// Use this for initialization
	void OnEnable () {
        origYPos = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		//if (isServer) {
			transform.Rotate (Vector3.up);
			transform.position = new Vector3 (transform.position.x, origYPos + Mathf.PingPong (Time.time, 1f), transform.position.z);
		//}
	}
}
