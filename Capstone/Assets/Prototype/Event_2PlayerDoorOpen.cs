using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Event_2PlayerDoorOpen : NetworkBehaviour {

	public int sugarToRelease = 10;
	public int requiredPlayers = 2;

//	public Event_ButtonPlayerDetection button1;
//	public Event_ButtonPlayerDetection button2;

	private int numPlayersOnButton = 0;
	private bool openingDoor = false;
	private Coroutine co;

	public Transform doorTarget;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void UpdatePlayersOnButton(int num)
	{
		numPlayersOnButton += num;

		if (numPlayersOnButton >= requiredPlayers && !openingDoor){
			if (!isServer) {
				return;
			} else {
				co = StartCoroutine (OpenDoor ());
			}
				}
		if (numPlayersOnButton < requiredPlayers && co != null)
		{
			openingDoor = false;
			StopCoroutine(co);
		}
	}

	private IEnumerator OpenDoor()
	{

		openingDoor = true;
		Vector3 origPos = transform.position;
	
		while(transform.position != doorTarget.position)
		{
			Debug.Log (transform.position);
			Debug.Log (doorTarget.position);
			transform.position = Vector3.MoveTowards(transform.position,doorTarget.position, Time.fixedDeltaTime*5);
			yield return null;
		}

		openingDoor = false;
	}
}
