using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnSceneChange : MonoBehaviour {

    static bool exists = false;

	// Use this for initialization
	void Awake () {

        if(exists)
        {
            Destroy(gameObject);
        }

        exists = true;
        GameObject.DontDestroyOnLoad(gameObject);	
	}
}
