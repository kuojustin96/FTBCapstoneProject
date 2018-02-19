using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class attack : NetworkBehaviour {
	public GameObject attackTrigger;
	public NetworkAnimator keyAnim;
	public bool attacking;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	if (!isLocalPlayer)
		return;
		if (Input.GetButtonDown ("Fire1")) {
			CmdAttacking ();

		}
	}
	[Command]
	//send attack to the server specifically
	//rpc isnt used here, because then rpc would need to be called twice, and rpc is less efficient than a cmd.
	public void CmdAttacking(){
		if(keyAnim.animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
		attackTrigger.SetActive (true);
		Invoke ("CmdStopAttacking", .4f);
		RpcAnimSend ();
			}
		 
	}
	[Command]
	public void CmdStopAttacking(){
		attackTrigger.SetActive (false);
		RpcAnimStop ();

	}
	[ClientRpc]
	//send anim to all clients
	public void RpcAnimSend(){
		keyAnim.animator.SetInteger ("keyAttack",1);
	}

	[ClientRpc]
	public void RpcAnimStop(){
		keyAnim.animator.SetInteger ("keyAttack", 0);
	}
}
