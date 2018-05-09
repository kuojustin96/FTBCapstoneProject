using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class fireAttackTrigger : NetworkBehaviour {
	PlayerClass player;
	public GameObject explosion;
	public GameObject flame;
	void Start(){
		Destroy (gameObject,5);
	}

	void OnTriggerEnter(Collider other){
		if(other.tag!="sugarPickup" && other.tag !="NetPlayer")
		{
			flame.SetActive (false);
			explosion.SetActive (true);
			Debug.Log (other);
			gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			Invoke ("destroyLate", .5f);
		}
	}

	public void destroyLate(){

	}

}

