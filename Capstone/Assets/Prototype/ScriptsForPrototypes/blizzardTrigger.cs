using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;

public class blizzardTrigger : NetworkBehaviour {
	public PlayerClass player;
	public GameObject parentPlayer;
    public float stunDuration = 4f;
    public float timer;
    private float curTimer;

	void OnEnable(){
		Debug.Log ("triggerActive");
		Invoke ("lateDestroy", 30f);
        timer = stunDuration * 2;
        curTimer = timer;
	}
	
	void OnTriggerStay(Collider other){

		if (other.tag == "NetPlayer"){
			Debug.Log(other);
			GameObject x = other.gameObject;

			if (x != parentPlayer) {
				stun (x);
			}

		}
	}

	public void stun(GameObject other){
        if (curTimer>timer)
        {
            other.GetComponent<net_PlayerController>().StunPlayerCoroutine(stunDuration);
            StatManager.instance.UpdateStat(Stats.TimeStunnedOthers, stunDuration);
            Debug.Log("stunCall");
            curTimer = 0;
        }
	}
	public void lateDestroy(){

		gameObject.SetActive (false);

		//player.currentItem.SetActive (false);
		player.currentItem = null;
//		gameObject.transform.root.gameObject.GetComponent<UIController> ().ResetUIItemTexture ();

	}
    private void FixedUpdate()
    {
        curTimer += Time.deltaTime;
    }
}
