using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sugarPickupable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnEnable(){
		
		Invoke ("lateEnable", 1f);
	}
	void lateEnable(){
		gameObject.GetComponent<BoxCollider> ().enabled = true; 

	}
}
