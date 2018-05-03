using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using jkuo;

public class attack : NetworkBehaviour {
	public GameObject attackTrigger;
	public GameObject fireballSpawn;
	public GameObject fireballPrefab;
	public NetworkAnimator keyAnim;
	//public NetworkAnimator matchAnim;
	public bool attacking;
	private PlayerClass player;
	public GameObject shooter;

	public GameObject magnetSize;

	public bool attackable;
	// Use this for initialization
	void Start () {
		 player = GetComponent<playerClassAdd>().player;
		attackable = true;
	}
	
	// Update is called once per frame
	void Update () {
	if (!isLocalPlayer)
		return;
		if (Input.GetButtonDown ("Fire1")&& attackable) {
			if (player.currentItem == null)
				return;
			if (player.currentItem.name == "keyHolder") {
				CmdKeyAttacking ();
				Debug.Log (player.currentItem.name);
				attackable = false;
				Invoke ("Attacking", 2);
			}
			if (player.currentItem.name == "matchHolder" ) {
				CmdMatchAttacking ();
				Debug.Log (player.currentItem.name);
				attackable = false;
				Invoke ("Attacking", 2);
			}
		}
	}

	public void Attacking(){
		attackable = true;
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
			Debug.Log ("keyaattack");
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
		if (player.currentItem != null && player.itemCharges <= 0) {

			CmdobjectTurnoff ();
		}
	}
	//Key

	//Match
	[Command]
	public void CmdMatchAttacking(){
		if (!player.isStunned) {
			//Invoke ("CmdFireball", 1.5f);
			CmdFireball ();
		}
		RpcMatchAnimSend ();
		Invoke ("CmdMatchStopAttacking", 2.458f);
	}

	[Command]
	public void CmdFireball(){
		Debug.Log ("fireball");
		GameObject fireball = Instantiate (fireballPrefab, fireballSpawn.transform.position, fireballSpawn.transform.rotation);
		NetworkServer.Spawn (fireball);
		RpcFireball (fireball);
	}
	[ClientRpc]
	public void RpcFireball(GameObject Fireball){
		Fireball.GetComponent<Rigidbody> ().AddForce(shooter.transform.forward * 10000);

	}

	[Command]
	public void CmdMatchStopAttacking(){
		RpcMatchAnimStop ();
	}
	[ClientRpc]
	//send anim to all clients
	public void RpcMatchAnimSend(){
		//matchAnim.animator.SetInteger ("matchAttack",1);
	}
	[ClientRpc]
	public void RpcMatchAnimStop (){
		//matchAnim.animator.SetInteger ("matchAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges <= 0) {

			CmdobjectTurnoff ();
		}
	}

	//Match


	//Magnet

	public void MagnetTurnOn(){

		magnetSize.GetComponent<SphereCollider> ().radius = 30;
		Invoke("MagnetTurnOff",40);
	}
	public void MagnetTurnOff(){

		magnetSize.GetComponent<SphereCollider> ().radius = 5;
		CmdobjectTurnoff ();
	}

	[Command]
	public void CmdobjectTurnoff(){
		RpcobjectTurnoff ();
	}
	[ClientRpc]
	public void RpcobjectTurnoff(){
		player.currentItem.SetActive (false);
		player.currentItem = null;
		GetComponent<UIController> ().ResetUIItemTexture ();

	}


}
