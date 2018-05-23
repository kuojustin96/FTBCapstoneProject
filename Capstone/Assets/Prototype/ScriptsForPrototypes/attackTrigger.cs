using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class attackTrigger : NetworkBehaviour {
	PlayerClass player;
    public float stunDuration = 4f;
    public ParticleSystem slashParticle;
    void OnEnable() { 
            Debug.Log ("triggerActive");
		}
	
		void OnTriggerEnter(Collider other){
			
				if (other.tag == "NetPlayer"){
				Debug.Log(other);
				GameObject x = other.gameObject;
                slashParticle.Play();
				stun (x);
			
			//Destroy (gameObject);
				}
			}
		
		public void stun(GameObject other){
		
		other.GetComponent<net_PlayerController> ().StunPlayerCoroutine(stunDuration);
        StatManager.instance.UpdateStat(Stats.TimeStunnedOthers, stunDuration);
        Debug.Log ("stunCall");
		}
	public void lateDestroy(){

		//Destroy (gameObject);
	}
}

