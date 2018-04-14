using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Trigger : MonoBehaviour {
	private PlayerClass player;
	private float sugarPickupSpeed;
	// Use this for initialization
    //Probaby want to move this to PlayerSugarPickup
	void Start () {
		player = GetComponentInParent<playerClassAdd>().player;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Dropoff Point")
		{
			//			player.crafttingMenuActive = true;
			if (player.dropoffPoint == other.gameObject) //If player owns this dropoff point
			{
				player.showCraftingUI = true;
				return;
			} 
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Dropoff Point")
		{
			player.showCraftingUI = false;

		}
	}
}
