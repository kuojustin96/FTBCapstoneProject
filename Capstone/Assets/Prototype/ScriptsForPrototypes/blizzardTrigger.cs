using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;

public class blizzardTrigger : NetworkBehaviour {
	public PlayerClass player;
	public GameObject parentPlayer;


	void OnEnable(){
		Debug.Log ("triggerActive");
		Invoke ("lateDestroy", 30f);
	}
	
	void OnTriggerEnter(Collider other){

		if (other.tag == "NetPlayer"){
			Debug.Log(other);
			GameObject x = other.gameObject;

			if (x != parentPlayer) {
				stun (x);
			}

		}
	}

	public void stun(GameObject other){

		other.GetComponent<net_PlayerController> ().StunPlayerCoroutine(4f);
		Debug.Log ("stunCall");
	}
	public void lateDestroy(){

		gameObject.SetActive (false);

		//player.currentItem.SetActive (false);
		player.currentItem = null;
//		gameObject.transform.root.gameObject.GetComponent<UIController> ().ResetUIItemTexture ();

	}
}
