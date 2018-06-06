using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class networkCallback : NetworkBehaviour {
	public PlayerSugarPickup sugarPickup;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void sugarCall(){
		if (!isLocalPlayer)
			return;
		CmdSugarCall ();
	}

	[Command]
	public void CmdSugarCall(){
		sugarPickup.StartCoroutine ("DropSugarAni");
		RpcSugarCall ();

	}
	[ClientRpc]
	public void RpcSugarCall(){
		sugarPickup.StartCoroutine ("DropSugarAni");

		}

	[Command]
	public void CmdSugarPickup(GameObject other){
		
		RpcSugarPickup (other);

	}
	[ClientRpc]
	public void RpcSugarPickup(GameObject other){
		sugarPickup.PickupSugarVoid(other);

	}
	}

