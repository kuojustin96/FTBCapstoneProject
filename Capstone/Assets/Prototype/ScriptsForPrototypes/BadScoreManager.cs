using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadScoreManager : MonoBehaviour {
	public Text p1Score;
	public Text p2Score;
	public Text p3Score;
	public Text p4Score;
	public GameObject gameManager;
	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("PlayerClassController");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (gameManager.GetComponent<GameManager> ().playerList [0] != null)
			p1Score.text = "Player 1: " + gameManager.GetComponent<GameManager> ().playerList [0].currentPlayerScore.ToString ();

		if (gameManager.GetComponent<GameManager> ().playerList.Count>1)
			p2Score.text ="Player 2: " + gameManager.GetComponent<GameManager> ().playerList [1].currentPlayerScore.ToString ();

		if (gameManager.GetComponent<GameManager> ().playerList.Count>2)
			p3Score.text ="Player 3: " + gameManager.GetComponent<GameManager> ().playerList [2].currentPlayerScore.ToString ();

		if (gameManager.GetComponent<GameManager> ().playerList.Count>3)
			p4Score.text = "Player 4: " +gameManager.GetComponent<GameManager> ().playerList [3].currentPlayerScore.ToString ();
	}
}
