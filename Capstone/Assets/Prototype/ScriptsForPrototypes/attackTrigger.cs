using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class attackTrigger : NetworkBehaviour {
	void Start(){
		Debug.Log ("triggerActive");
	}

	void OnTriggerEnter(Collider other){
		
			if (other.tag == "NetPlayer"){
			Debug.Log(other);
			//other.GetComponent<net_PlayerController> ().StunPlayer (10f);

			RpcStun (other);
			}
		}
	[ClientRpc]
	void RpcStun(Collider other){
		other.GetComponent<net_PlayerController> ().StunPlayer (10f);
	}
	}

