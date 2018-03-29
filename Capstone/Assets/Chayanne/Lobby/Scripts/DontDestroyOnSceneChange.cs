using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnSceneChange : MonoBehaviour {

    static bool exists = false;

    //If the item is already there, Destroy this.
    void Awake () {
        if(exists)
        {
            Destroy(gameObject);
        }

        exists = true;
        GameObject.DontDestroyOnLoad(gameObject);	
	}
}
