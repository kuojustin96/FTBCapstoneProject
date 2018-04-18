using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ckp;
public class playerClassAdd : MonoBehaviour {
	public PlayerClass player;
	public PlayerSugarPickup psp;
	public GameObject Hood;
	public GameObject Cloak;
	// Use this for initialization
	void Start () {
		if (gameObject.tag != "LobbyPlayer") {
			GameObject.Find ("PlayerClassController").GetComponent<GameManager> ().SetUpGame (this.gameObject, gameObject.GetComponent<net_TeamScript> ().teamColor);
		}
	}

    public void SetName(string name)
    {

    }
}
