using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Event_ButtonPlayerDetection : NetworkBehaviour {

	private Event_2PlayerDoorOpen doorOpen;
	public GameObject door;

	// Use this for initialization
	void Start () {
		doorOpen = door.GetComponent<Event_2PlayerDoorOpen>();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "NetPlayer")
			doorOpen.UpdatePlayersOnButton(1);
		Debug.Log ("PlayerOnButton");	
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "NetPlayer")
			doorOpen.UpdatePlayersOnButton(-1);
	}
}