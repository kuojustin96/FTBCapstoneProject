using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class fireProjectileTrigger : NetworkBehaviour {
	PlayerClass player;
    public float stunDuration = 4f;
    public GameObject sentPlayer;
	void Start(){
		Debug.Log ("triggerActive");
		Invoke ("lateDestroy", 1.5f);
	}

	void OnTriggerEnter(Collider other){

		if (other.tag == "NetPlayer"){
			Debug.Log(other);
			//other.GetComponent<net_PlayerController> ().StunPlayer (10f);
			GameObject x = other.gameObject;
            if(x!= transform.parent.gameObject.GetComponent<fireAttackTrigger>().sentPlayer)
			stun (x);

		}
	}

	public void stun(GameObject other){

		other.GetComponent<net_PlayerController> ().StunPlayerCoroutine(stunDuration);
        StatManager.instance.UpdateStat(Stats.TimeStunnedOthers, stunDuration);
        Debug.Log ("stunCall");
	}
	public void lateDestroy(){

		Destroy (gameObject.transform.root.gameObject);
	}
}

