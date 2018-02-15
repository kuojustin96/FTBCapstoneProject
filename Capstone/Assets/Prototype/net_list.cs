using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class net_list : NetworkBehaviour {
	public List<GameObject> buttons = new List<GameObject>();
	public  List<int>  buttonBool = new List<int>();
	public static int triggersActive;
	public int iterations;
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")){
			Debug.Log ("serverCheck");
			if (isServer) {
				//if(triggersActive == 3){
				Debug.Log ("new");
				int count = 0;
				string listString = "";
				while (count < 5) {
					buttonBool [count] = (int)Random.Range (0, 2);	
					listString = listString + buttonBool [count].ToString ();
					count++;
				}
					RpcColorRandomize (listString);
				//}
			}
		}
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
}
