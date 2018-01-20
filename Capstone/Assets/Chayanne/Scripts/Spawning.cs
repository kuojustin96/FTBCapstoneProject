using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawning : MonoBehaviour {
	public Transform Spawn1;
	public Transform Spawn2;
	public bool spawned1;
	// Use this for initialization
	void Start () {
		if (NetManager.spawned1 == false) {
			gameObject.transform.position = Spawn1.position;
			NetManager.spawned1 = true;

		} else
			gameObject.transform.position = Spawn2.position;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
