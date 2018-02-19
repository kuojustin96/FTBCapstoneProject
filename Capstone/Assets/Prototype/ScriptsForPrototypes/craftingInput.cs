using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class craftingInput : NetworkBehaviour {
	private PlayerClass player;
	public GameObject FToCraft;
	public GameObject CraftingUI;
	public GameObject FullCraftingUI;
	public List<GameObject> attackItems = new List<GameObject>();
	

	void Start(){
		player = GetComponent<playerClassAdd>().player;
	}
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		if (player.crafttingMenuActive)
			FullCraftingUI.SetActive (true);

		if (!player.crafttingMenuActive)
			FullCraftingUI.SetActive (false);

		if (Input.GetKeyDown (KeyCode.F) && player.crafttingMenuActive) {
			FToCraft.SetActive (false);
			CraftingUI.SetActive (true);
		}
	}
	public void craftAttack(){
		if (!isLocalPlayer)
			return;
		CmdCraftAttack ();
	}

	[Command]
	public void CmdCraftAttack(){
		RpcCraftAttack ();
		attackItems [0].SetActive (true);
	}
	[ClientRpc]
	public void RpcCraftAttack (){
		attackItems [0].SetActive (true);
	}
}
