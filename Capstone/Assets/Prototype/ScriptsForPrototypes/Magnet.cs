using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		GetComponentInParent<attack> ().MagnetTurnOn ();
	}
	

}
