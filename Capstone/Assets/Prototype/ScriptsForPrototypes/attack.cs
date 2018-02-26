using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using jkuo;

public class attack : NetworkBehaviour {
	public GameObject attackTrigger;
	public NetworkAnimator keyAnim;
	public bool attacking;
	private PlayerClass player;
	public GameObject key;
	// Use this for initialization
	void Start () {
		 player = GetComponent<playerClassAdd>().player;
	}
	
	// Update is called once per frame
	void Update () {
	if (!isLocalPlayer)
		return;
		if (Input.GetButtonDown ("Fire1")) {
			if (player.currentItem == null)
				return;
			if (player.currentItem.name == "keyHolder") {
				CmdAttacking ();
				Debug.Log (player.currentItem.name);
			}
		}
	}
	[Command]
	//send attack to the server specifically
	//rpc isnt used here, because then rpc would need to be called twice, and rpc is less efficient than a cmd.
	public void CmdAttacking(){
		
		if(keyAnim.animator.GetCurrentAnimatorStateInfo(0).IsName("idle")&& !player.isStunned ){
		attackTrigger.SetActive (true);
		Invoke ("CmdStopAttacking", .5f);
		RpcAnimSend ();
			}
		 
	}
	[Command]
	public void CmdStopAttacking(){
		RpcAnimStop ();

	}
	[ClientRpc]
	//send anim to all clients
	public void RpcAnimSend(){
		attackTrigger.SetActive (true);
		keyAnim.animator.SetInteger ("keyAttack",1);
	}

	[ClientRpc]
	public void RpcAnimStop(){
		attackTrigger.SetActive (false);
		keyAnim.animator.SetInteger ("keyAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges == 0) {

			CmdobjectTurnoff ();
		}
	}


	[Command]
	public void CmdobjectTurnoff(){
		RpcobjectTurnoff ();
	}
	[ClientRpc]
	public void RpcobjectTurnoff(){
		player.currentItem.SetActive (false);
		player.currentItem = null;

	}
}
