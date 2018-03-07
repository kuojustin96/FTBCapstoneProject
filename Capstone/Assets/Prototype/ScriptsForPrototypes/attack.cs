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
	public GameObject firePrefab;
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
				CmdKeyAttacking ();
				Debug.Log (player.currentItem.name);
			}
			if (player.currentItem.name == "matchHolder") {
				CmdMatchAttacking ();
			}
		}
	}

	//KEY
	[Command]
	//send attack to the server specifically
	//rpc isnt used here, because then rpc would need to be called twice, and rpc is less efficient than a cmd.
	public void CmdKeyAttacking(){
		
		if(keyAnim.animator.GetCurrentAnimatorStateInfo(0).IsName("idle")&& !player.isStunned ){
		attackTrigger.SetActive (true);
		Invoke ("CmdKeyStopAttacking", .5f);
		RpcKeyAnimSend ();
			}
		 
	}
	[Command]
	public void CmdKeyStopAttacking(){
		RpcKeyAnimStop ();

	}
	[ClientRpc]
	//send anim to all clients
	public void RpcKeyAnimSend(){
		attackTrigger.SetActive (true);
		keyAnim.animator.SetInteger ("keyAttack",1);
	}

	[ClientRpc]
	public void RpcKeyAnimStop(){
		attackTrigger.SetActive (false);
		keyAnim.animator.SetInteger ("keyAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges == 0) {

			CmdobjectTurnoff ();
		}
	}
	//Key

	//Matchstick
	[Command]
	public void CmdMatchAttacking(){


		CmdMatchFire ();
	}
	[Command]
	public void CmdMatchFire(){

		GameObject fire = Instantiate(firePrefab, this.transform.position, Quaternion.identity);
		NetworkServer.Spawn (fire);

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
