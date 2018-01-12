using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAllThisShit : MonoBehaviour {

    public GameObject[] gameObjectList;

	// Use this for initialization
	void Start () {
		
        foreach (GameObject meme in gameObjectList)
        {
            meme.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
