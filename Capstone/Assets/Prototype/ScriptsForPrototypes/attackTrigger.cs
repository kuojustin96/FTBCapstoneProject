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
				GameObject x = other.gameObject;
				stun (x);
				}
			}
		
		public void stun(GameObject other){
			other.GetComponent<net_PlayerController> ().RpcStunPlayer (10f);
			Debug.Log ("stunCall");
		}
}

