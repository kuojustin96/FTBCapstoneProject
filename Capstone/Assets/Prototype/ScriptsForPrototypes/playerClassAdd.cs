﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ckp;
public class playerClassAdd : MonoBehaviour {
	public PlayerClass player;
	public PlayerSugarPickup psp;
	public GameObject Hood;
	public GameObject Cloak;
    public GameManager gm;
	// Use this for initialization
	void Start () {
		if (gameObject.tag != "LobbyPlayer") {
            gm = GameManager.instance;
            gm.SetUpGame (this.gameObject, gameObject.GetComponent<net_TeamScript>().teamColor);
            //GameManager.instance.SetUpGame(this.gameObject, gameObject.GetComponent<net_TeamScript>().teamColor);
            //gm.SetUpGame(this.gameObject, gameObject.GetComponent<net_TeamScript>().teamColor);
        }
	}

    public void SetName(string name)
    {

    }
}
