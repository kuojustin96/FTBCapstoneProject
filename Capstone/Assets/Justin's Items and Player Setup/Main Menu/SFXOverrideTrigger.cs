using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXOverrideTrigger : MonoBehaviour {

    public string SFXOverrideName;

	// Use this for initialization
	void Start () {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
	}
}