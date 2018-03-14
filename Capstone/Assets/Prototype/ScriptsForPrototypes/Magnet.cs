using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponentInParent<attack> ().MagnetTurnOn ();
	}
	

}
