﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;
public class FindBullshit : MonoBehaviour {

    public AudioListener[] memes;

    // Use this for initialization
    void Start () {




	}
	
	// Update is called once per frame
	void Update () {
        memes = GameObject.FindObjectsOfType<AudioListener>();
    }
}