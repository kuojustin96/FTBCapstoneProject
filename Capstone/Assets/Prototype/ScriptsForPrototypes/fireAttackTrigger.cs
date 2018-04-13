using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class fireAttackTrigger : NetworkBehaviour {
	PlayerClass player;
	void Start(){
		Debug.Log ("triggerActive");
		//Invoke ("lateDestroy", 5);
	}

	void OnTriggerEnter(Collider other){

		if (other.tag == "NetPlayer"){
			Debug.Log(other);
			//other.GetComponent<net_PlayerController> ().StunPlayer (10f);
			GameObject x = other.gameObject;
			stun (x);

			//Destroy (gameObject);
		}
	}

	public void stun(GameObject other){

		other.GetComponent<net_PlayerController> ().StunPlayerCoroutine(4f);
		Debug.Log ("stunCall");
	}
	public void lateDestroy(){

		//Destroy (gameObject);
	}
}

