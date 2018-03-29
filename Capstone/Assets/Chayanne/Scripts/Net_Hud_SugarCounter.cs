using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Net_Hud_SugarCounter : MonoBehaviour {

	public TextMeshProUGUI sugarInBackpackText;
	public TextMeshProUGUI stashText;
	private GameObject playerGO;
	public PlayerClass player;
	// Use this for initialization
	void Start () {
		//tm = GetComponent<TextMeshProUGUI> ();
	}

	void Update(){
		sugarInBackpackText.text = player.sugarInBackpack.ToString();
		stashText.text = player.currentPlayerScore.ToString ();
	}

}
