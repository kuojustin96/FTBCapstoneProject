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
	public List<GameObject> defenseItems = new List<GameObject>();

	public List<int> attackCharges = new List<int>();
	public List<int> defenseCharges = new List<int>();

	

	void Start(){
		player = GetComponent<playerClassAdd>().player;
	}
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		//if (player.crafttingMenuActive)
		//	FullCraftingUI.SetActive (true);

		//if (!player.crafttingMenuActive)
		//	FullCraftingUI.SetActive (false);

		//if (Input.GetKeyDown (KeyCode.F) && player.crafttingMenuActive) {
		//	FToCraft.SetActive (false);
		//	CraftingUI.SetActive (true);
		//}
	}
	public void craftAttack(){
		if (!isLocalPlayer)
			return;
		if (player.currentItem != null)
			return;
		if (player.currentPlayerScore > 0) {
			int randomRange = Random.Range (0, attackItems.Count);
			CmdCraftAttack (randomRange);
			player.itemCharges = attackCharges [randomRange];
			player.currentItem = attackItems [randomRange];
			player.currentItemString = player.currentItem.ToString ();
		}
	}

	[Command]
	public void CmdCraftAttack(int randomRange){
		RpcCraftAttack (randomRange);
		attackItems [randomRange].SetActive (true);
		player.currentItem = attackItems [randomRange];
		player.currentItemString = player.currentItem.ToString();

	}
	[ClientRpc]
	public void RpcCraftAttack (int randomRange){
		attackItems [randomRange].SetActive (true);
		player.currentItem = attackItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
		player.currentPlayerScore -= 1;
	}

	public void craftDefense(){
		if (!isLocalPlayer)
			return;
		if (player.currentItem != null)
			return;
		if (player.currentPlayerScore > 0) {
			int randomRange = Random.Range (0, defenseItems.Count);
			CmdCraftDefense (randomRange);
			player.itemCharges = defenseCharges [randomRange];
			player.currentItem = defenseItems [randomRange];
			player.currentItemString = player.currentItem.ToString ();
		}
	}
	[Command]
	public void CmdCraftDefense(int randomRange){
		RpcCraftDefense (randomRange);
		defenseItems [randomRange].SetActive (true);
		player.currentItem = defenseItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
	}
	[ClientRpc]
	public void RpcCraftDefense(int randomRange){
		defenseItems [randomRange].SetActive (true);
		player.currentItem = defenseItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
		player.currentPlayerScore--;
	}

}
