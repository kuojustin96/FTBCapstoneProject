using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class fireAttackTrigger : NetworkBehaviour {
	PlayerClass player;
	public GameObject explosion;
	public GameObject flame;
    private NetworkSoundController nsc;
	void Start(){
        nsc = GetComponent<NetworkSoundController>();
		Destroy (gameObject,5);
	}

	void OnTriggerEnter(Collider other){
		if(other.tag!="sugarPickup" && other.tag !="NetPlayer")
		{
			flame.SetActive (false);
			explosion.SetActive (true);
            nsc.CmdPlaySFX("FireballImpact", gameObject, 1f, 200f, true, false);
			gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			Invoke ("destroyLate", .5f);
		}
	}

	public void destroyLate(){

	}

}

