using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ckp;

[RequireComponent(typeof(net_Event_MultiplayerTrigger))]
public class net_list : NetworkBehaviour {
	public List<GameObject> buttons = new List<GameObject>();
	public  List<int>  buttonBool = new List<int>();
	public static int triggersActive;
	public int iterations;

	private net_Event_MultiplayerTrigger eventTrigger;

	void Start(){
		eventTrigger = GetComponent<net_Event_MultiplayerTrigger> ();
		randomizeTwo ();



		//If you get down here, then you know all the correct buttons are pressed
	}

	// Update is called once per frame
	void Update (){
		


	}
	[ClientRpc]
	public void RpcColorRandomize(string x){
		Debug.Log (x);
		int counta = 0;
		if (iterations == 4) {
			//Bring in a lot of sugar//
		} else {
			while (counta < 5) {	
				Debug.Log (x [counta] + "   test");
				if (x [counta].ToString () == 1.ToString ())
					buttons [counta].GetComponent<MeshRenderer> ().material.color = Color.cyan;
				if (x [counta].ToString () == 0.ToString ())
					buttons [counta].GetComponent<MeshRenderer> ().material.color = Color.grey;
				counta++;
			}
		}
	}

	public void randomizeTwo(){

		Debug.Log ("Randomize");
		if (isServer) {
			//if(triggersActive == 3){

			int count = 0;
			int actives = 0;
			string listString = "";
			while (count < 5) {
				if (actives == 2) {
					buttonBool[count] = 0;
				} else {
					buttonBool [count] = (int)Random.Range (0, 2);
				}
				listString = listString + buttonBool [count].ToString ();
				if (buttonBool [count] == 1) {
					actives++;
				}
				count++;
			}
			if (actives == 2) {
				RpcColorRandomize (listString);
			}
			else{
				randomizeTwo ();
			}
			//}

		}
	}
	public void checkCompletion(){

		Debug.Log ("completion method");
		for(int x = 0; x < buttonBool.Count; x++) {
			if (buttonBool[x] == 1) {
				if (!eventTrigger.triggers [x].IsTriggered())
					return;
			}
		}
		randomizeTwo ();
	}
}
