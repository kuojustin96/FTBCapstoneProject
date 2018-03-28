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
	public List<GameObject> utilityItems = new List<GameObject> ();


	public List<int> attackCharges = new List<int>();
	public List<int> defenseCharges = new List<int>();
	public List<int> utilityCharges = new List<int> ();

	

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

		if (player.currentPlayerScore > 0) {
			if (player.currentItem != null)
				player.currentItem.SetActive (false);
			
			player.currentItem = null;
			int randomRange = Random.Range (0, attackItems.Count);
			CmdCraftAttack (randomRange);
			player.itemCharges = attackCharges [randomRange];
			player.currentItem = attackItems [randomRange];
			player.currentItemString = player.currentItem.ToString ();
		}
	}	

	[Command]
	public void CmdCraftAttack(int randomRange){
		if (player.currentItem != null)
			player.currentItem.SetActive (false);
		
		player.currentItem = null;
		RpcCraftAttack (randomRange);
		attackItems [randomRange].SetActive (true);
		player.currentItem = attackItems [randomRange];
		player.currentItemString = player.currentItem.ToString();

	}
	[ClientRpc]
	public void RpcCraftAttack (int randomRange){
		if (player.currentItem != null)
			player.currentItem.SetActive (false);
		
		player.currentItem = null;
		attackItems [randomRange].SetActive (true);
		player.currentItem = attackItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
		player.currentPlayerScore -= 1;
		GetComponent<UIController> ().UpdateStashUI (player.currentPlayerScore);
	}

	public void craftDefense(){
		if (!isLocalPlayer)
			return;
		if (player.currentPlayerScore > 0) {
			if (player.currentItem != null)
				player.currentItem.SetActive (false);
			
			player.currentItem = null;
			int randomRange = Random.Range (0, defenseItems.Count);
			CmdCraftDefense (randomRange);
			player.itemCharges = defenseCharges [randomRange];
			player.currentItem = defenseItems [randomRange];
			player.currentItemString = player.currentItem.ToString ();
		}
	}
	[Command]
	public void CmdCraftDefense(int randomRange){
		if (player.currentItem != null)
			player.currentItem.SetActive (false);
		
		player.currentItem = null;
		RpcCraftDefense (randomRange);
		defenseItems [randomRange].SetActive (true);
		player.currentItem = defenseItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
	}
	[ClientRpc]
	public void RpcCraftDefense(int randomRange){
		if (player.currentItem != null)
			player.currentItem.SetActive (false);
		
		player.currentItem = null;
		defenseItems [randomRange].SetActive (true);
		player.currentItem = defenseItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
		player.currentPlayerScore--;
		GetComponent<UIController> ().UpdateStashUI (player.currentPlayerScore);
	}

	public void craftUtility(){
		if (!isLocalPlayer)
			return;
		if (player.currentPlayerScore > 0) {
			if (player.currentItem != null)
				player.currentItem.SetActive (false);
			
			player.currentItem = null;
			int randomRange = Random.Range (0, utilityItems.Count);
			CmdCraftUtility (randomRange);
			player.itemCharges = utilityCharges [randomRange];
			player.currentItem = utilityItems [randomRange];
			player.currentItemString = player.currentItem.ToString ();
		}
	}
	[Command]
	public void CmdCraftUtility(int randomRange){
		if (player.currentItem != null)
			player.currentItem.SetActive (false);
		
		player.currentItem = null;
		RpcCraftUtility (randomRange);
		utilityItems [randomRange].SetActive (true);
		player.currentItem = utilityItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
	}
	[ClientRpc]
	public void RpcCraftUtility(int randomRange){
		if (player.currentItem != null)
			player.currentItem.SetActive (false);
		
		player.currentItem = null;
		utilityItems [randomRange].SetActive (true);
		player.currentItem = utilityItems [randomRange];
		player.currentItemString = player.currentItem.ToString();
		player.currentPlayerScore--;
		GetComponent<UIController> ().UpdateStashUI (player.currentPlayerScore);
	}


}
