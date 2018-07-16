using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysEnableObject : MonoBehaviour {

    public GameObject[] gameObj;
	
	// Update is called once per frame
	void Update () {

        foreach(GameObject obj in gameObj)
        {
            obj.SetActive(true);
        }

	}
}
