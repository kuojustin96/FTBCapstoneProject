using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour {
	public float rotateSpeed;
    public float rotateSpeedx;
    public float rotateSpeedz;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate (rotateSpeedx, rotateSpeed, rotateSpeedz);
	}
}
