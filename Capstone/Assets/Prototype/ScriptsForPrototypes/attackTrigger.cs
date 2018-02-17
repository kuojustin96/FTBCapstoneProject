using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackTrigger : MonoBehaviour {
	void Start(){
		Debug.Log ("triggerActive");
	}

	void OnTriggerEnter(Collider other){
		
			if (other.tag == "NetPlayer"){
			Debug.Log(other);
			}
		}
	}

