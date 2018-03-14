using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class BadScoreManager : NetworkBehaviour {
	public Text p1Score;
	public Text p2Score;
	public Text p3Score;
	public Text p4Score;
	public GameObject gameManager;
	public int winAmount;
	public GameObject WinUI;
	public Text winText;
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
		gameManager = GameObject.Find ("PlayerClassController");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (gameManager != null) {
			if (gameManager.GetComponent<GameManager> ().playerList [0] != null) {
				if (gameManager.GetComponent<GameManager> ().playerList [0].currentPlayerScore >= winAmount)
					Win ();
				winText.text = "Player 1 Wins";
			}
			

			if (gameManager.GetComponent<GameManager> ().playerList.Count > 1) {
				if (gameManager.GetComponent<GameManager> ().playerList [1].currentPlayerScore >= winAmount)
					Win ();
				winText.text = "Player 2 Wins";
			}
			if (gameManager.GetComponent<GameManager> ().playerList.Count > 2) {
				if (gameManager.GetComponent<GameManager> ().playerList [2].currentPlayerScore >= winAmount)
					Win ();
				winText.text = "Player 3 Wins";
			}
			if (gameManager.GetComponent<GameManager> ().playerList.Count > 3) {
				if (gameManager.GetComponent<GameManager> ().playerList [3].currentPlayerScore >= winAmount)
					Win ();
				winText.text = "Player 4 Wins";
			}
		}
	}
	public void Win(){
		SceneManager.LoadScene ("winScene");
		WinUI.SetActive (true);
	}

	[Command]
	public void CmdSceneSwap(){
		RpcSceneSwap ();
	}
	[ClientRpc]
	public void RpcSceneSwap(){
		SceneManager.LoadScene ("winScene");
	}
}