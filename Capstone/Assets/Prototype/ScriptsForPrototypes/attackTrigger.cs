using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;
public class attackTrigger : NetworkBehaviour {
	PlayerClass player;
    public GameObject parentPlayer;
    public float stunDuration = 4f;
    public ParticleSystem slashParticle;
    public ParticleSystem Explosion;
    public ParticleSystem shards;
    void OnEnable() { 
            Debug.Log ("triggerActive");
		}
    private void Start()
    {
        slashParticle.Play();
        Invoke("lateDestroy", 2f);
    }

    void OnTriggerEnter(Collider other){
			
				if (other.tag == "NetPlayer"){
				Debug.Log(other);
				GameObject x = other.gameObject;
                
            if (x != parentPlayer)
            {
                stun(x);
                Explosion.transform.position = other.transform.position;
                shards.transform.position = other.transform.position;
                Explosion.Play();
                shards.Play();
            }
            
			
			//Destroy (gameObject);
				}
			}
		
		public void stun(GameObject other){
		
		other.GetComponent<net_PlayerController> ().StunPlayerCoroutine(stunDuration);
        StatManager.instance.UpdateStat(Stats.TimeStunnedOthers, stunDuration);
        Debug.Log ("stunCall");
		}
	public void lateDestroy(){

		Destroy (gameObject);
	}
}

