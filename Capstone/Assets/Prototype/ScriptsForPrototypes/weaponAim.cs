using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAim : MonoBehaviour {
	public Transform mainCam;
	// Use this for initialization
	void Start () {
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.rotation = mainCam.rotation;
	}
}
