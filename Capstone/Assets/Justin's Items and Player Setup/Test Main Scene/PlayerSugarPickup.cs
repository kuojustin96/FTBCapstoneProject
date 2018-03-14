using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSugarPickup : MonoBehaviour {

	private PlayerClass player;
	private float sugarPickupSpeed;
	private float dropoffDelay;
	private List<GameObject> sugarInBackpack = new List<GameObject>();
	private bool runAnimation = false;

    //	//PROTOTYPE HUD
    //public Net_Hud_SugarCounter hudCount;

    private UIController uiController;

	// Use this for initialization
	void Start () {
		player = GetComponentInParent<playerClassAdd>().player;
        //uiController = GameObject.Find("Player UI Canvas").GetComponent<UIController>();
		uiController = GetComponentInParent<UIController>();

        //uiController.SetUpVariables(player);

        //player.SetUIController(uiController);
		sugarPickupSpeed = GameManager.instance.sugarPickUpSpeed;
		dropoffDelay = GameManager.instance.dropoffDelay;
	}

	// Update is called once per frame
	void Update () {
		if (!player.isStunned)
		{
			if (Input.GetKeyDown(KeyCode.Q))
				DropSugar();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "SugarCube")
		{
			if(player.sugarInBackpack < player.maxCanCarry)
				StartCoroutine(PickupSugarAni(other.gameObject));
			return;
		}

		if (other.tag == "Dropoff Point")
		{
			//			player.crafttingMenuActive = true;
			runAnimation = true;
			if (player.dropoffPoint == other.gameObject) //If player owns this dropoff point
			{
				//player.showCraftingUI = true;
				runAnimation = true;
				//Debug.Log(sugarInBackpack.Count);
				if (sugarInBackpack.Count > 0)
				{
					//runAnimation = true;
					StartCoroutine(DropoffSugarAni(other.gameObject));
				}
				return;
			} else //If player does not own this dropoff point
			{
				Debug.Log (other.transform.parent.gameObject);
				StartCoroutine(StealSugarAni(other.gameObject));
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Dropoff Point")
		{
			//player.showCraftingUI = false;
			runAnimation = false;
			//if (player.dropoffPoint == other.gameObject)
			//{
			//    runAnimation = false;
			//    return;
			//}
		}
	}

	public void SugarTransport(int cost)
	{
		if (sugarInBackpack.Count > 0)
		{
			if (cost == 0)
				cost = 1;

			Debug.Log(cost);
			GameObject dropoffPoint = GameManager.instance.DropoffPoints[player.playerNum].dropoffGO;
			for (int x = 0; x < cost; x++)
			{
				//SugarManager.instance.DisableSugar(sugarInBackpack[0]);
				sugarInBackpack.Remove(sugarInBackpack[0]);
				player.DropSugar();
			}

			while(sugarInBackpack.Count > 0)
			{
				player.DropoffSugarInStash();
				sugarInBackpack[0].transform.parent = dropoffPoint.transform;
				sugarInBackpack[0].transform.position = Vector3.zero;
				sugarInBackpack.Remove(sugarInBackpack[0]);
			}
		}
	}

	public void DropSugar()
	{
		if(sugarInBackpack.Count > 0)
		{
			//Breaks if you spam the drop button
			StartCoroutine(DropSugarAni());
		}
	}

	public void StunDropSugar()
	{
		if (sugarInBackpack.Count > 0)
		{
			int dropAmount;
			if (sugarInBackpack.Count == 1)
				dropAmount = 1;
			else
				dropAmount = sugarInBackpack.Count / 2;

			for (int x = 0; x < dropAmount; x++)
			{
				StartCoroutine(DropSugarAni());
			}
		}
	}

	private IEnumerator DropSugarAni()
	{
        //Drop sugar on the ground

		player.DropSugar();
        uiController.UpdateBackpackScore(player.sugarInBackpack);
        int count = 0;
		GameObject sugar = sugarInBackpack[0];
		sugar.SetActive(true);
		sugarInBackpack.Remove(sugarInBackpack[0]);
		sugar.transform.parent = null;
		Vector3 topPos = new Vector3(sugar.transform.position.x, sugar.transform.position.y + 1, sugar.transform.position.z);

		while (count < 10)
		{
			count++;
			sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
			yield return null;
		}

		Vector3 randomDropLoc = transform.parent.position + Random.onUnitSphere * 15f;
		randomDropLoc.y = transform.parent.position.y;
		count = 0;

		while(sugar.transform.position != randomDropLoc)
		{
			count++;
			sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, randomDropLoc, sugarPickupSpeed * 2);
			yield return null;
		}

		sugar.GetComponent<SimpleRotate>().enabled = true;
		sugar.GetComponent<BoxCollider>().enabled = true;
	}

	private IEnumerator StealSugarAni(GameObject dropoffPoint)
	{
        //Steal sugar from enemy stash

		int count = 0;

		PlayerClass otherPlayer = GameManager.instance.GetPlayerFromDropoff(dropoffPoint);

		if (otherPlayer.currentPlayerScore > 0)
		{
			GameObject sugar = otherPlayer.dropoffPoint.transform.parent.GetChild(1).gameObject;
			sugarInBackpack.Add(sugar);
			otherPlayer.LoseSugar(1);
            player.PickupSugar();

            uiController.UpdateBackpackScore(player.sugarInBackpack);
            otherPlayer.playerGO.GetComponent<UIController>().UpdateStashUI(otherPlayer.currentPlayerScore);

            Vector3 topPos = new Vector3(dropoffPoint.transform.position.x, dropoffPoint.transform.position.y + 2, dropoffPoint.transform.position.z);
			sugar.transform.parent = null;
			sugar.SetActive(true);
			Vector3 saveScale = sugar.transform.localScale;
//			while (count < 20)
//			{
//				count++;
//				sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
//				yield return null;
//			}
//
//			count = 0;

			while (count < 20)
			{
				count++;
				sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
				sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, transform.position, sugarPickupSpeed);
				yield return new WaitForFixedUpdate ();
			}

			sugar.SetActive(false);
			sugar.transform.localScale = saveScale; //Reset sugar scale, might need to change later
			sugar.transform.parent = transform;
			sugar.transform.position = transform.position;

            if (otherPlayer.currentPlayerScore > 0 && runAnimation)
            {
                yield return new WaitForSeconds(2f);
                StartCoroutine(StealSugarAni(dropoffPoint));
            }
		}
	}

	private IEnumerator PickupSugarAni(GameObject sugar)
	{
        //Pick up sugar from the ground

		//SugarManager.instance.CmdEnableNewSugar(sugar);

		sugar.GetComponent<SimpleRotate>().enabled = false;
		sugar.GetComponent<BoxCollider>().enabled = false;
		player.PickupSugar();
		sugarInBackpack.Add(sugar);
        uiController.UpdateBackpackScore(player.sugarInBackpack);

        sugar.transform.parent = transform;
		Vector3 saveScale = sugar.transform.localScale;

		while (sugar.transform.position != transform.position)
		{
			sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
			sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, transform.position, sugarPickupSpeed);
			yield return null;
		}

		sugar.SetActive(false);
		sugar.transform.localScale = saveScale;

	}

	private IEnumerator DropoffSugarAni(GameObject dropoffPoint)
	{
        //Drop off sugar in the player's stash
		int count = 0;

		Vector3 saveScale = sugarInBackpack[0].transform.localScale;
		GameObject sugar = sugarInBackpack[0];
		sugarInBackpack.Remove(sugarInBackpack[0]);
		sugar.transform.parent = null;
		player.DropoffSugarInStash();

        uiController.UpdateBackpackScore(player.sugarInBackpack);
        uiController.UpdateStashUI(player.currentPlayerScore);

		sugar.SetActive(true);

		Vector3 topPos = new Vector3(sugar.transform.position.x, sugar.transform.position.y + 10, sugar.transform.position.z);

		while(count < 10)
		{
			count++;
			sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed*10);
			yield return new WaitForFixedUpdate ();
		}

		count = 0;

		while (count < 10)
		{
			count++;
			sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
			sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, new Vector3( dropoffPoint.transform.position.x, dropoffPoint.transform.position.y+50, dropoffPoint.transform.position.z) , sugarPickupSpeed);
			yield return new WaitForFixedUpdate ();
		}

		sugar.SetActive(false);
		sugar.transform.localScale = saveScale;
		sugar.transform.parent = dropoffPoint.transform.parent;
		sugar.transform.position = new Vector3 (dropoffPoint.transform.position.x, dropoffPoint.transform.position.y + 50, dropoffPoint.transform.position.z);

		yield return new WaitForSeconds(dropoffDelay);

		if (sugarInBackpack.Count > 0 && runAnimation)
			StartCoroutine(DropoffSugarAni(dropoffPoint));
	}
}