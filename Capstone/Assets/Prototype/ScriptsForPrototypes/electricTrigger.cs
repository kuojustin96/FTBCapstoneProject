using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using jkuo;

public class electricTrigger : NetworkBehaviour {
	PlayerClass player;
	GameObject parentPlayer;
    private NetworkSoundController nsc;
    public float stunDuration = 4f;

    private void Awake()
    {
        nsc = transform.root.GetComponent<NetworkSoundController>();
    }

    void OnEnable(){
		player = gameObject.transform.root.gameObject.GetComponent<playerClassAdd>().player;
		Debug.Log ("triggerActive");
		parentPlayer = gameObject.transform.root.gameObject;
        nsc.CmdPlaySFX("Battery", transform.root.gameObject, 1f, 100f, false, true);
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

		other.GetComponent<net_PlayerController> ().StunPlayerCoroutine(stunDuration);
        StatManager.instance.UpdateStat(Stats.TimeStunnedOthers, stunDuration);
        Debug.Log ("stunCall");
	}
	public void lateDestroy(){

		gameObject.SetActive (false);
        nsc.CmdStopSFX("Battery", transform.root.gameObject);
        //player.currentItem.SetActive (false);
        player.currentItem = null;
		gameObject.transform.root.gameObject.GetComponent<UIController> ().ResetUIItemTexture ();

	}
}
