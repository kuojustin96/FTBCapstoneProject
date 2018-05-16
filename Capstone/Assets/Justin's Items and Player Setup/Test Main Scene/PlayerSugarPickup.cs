using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jkuo;
using DG.Tweening;
using UnityEngine.Networking;


public class PlayerSugarPickup : NetworkBehaviour {

	private PlayerClass player;
    private float sugarPickupTime;
	private float dropoffDelay;
	private List<GameObject> sugarInBackpack = new List<GameObject>();
	private bool runAnimation = false;
	public bool dropping = false;
	public networkCallback callback;

    private UIController uiController;
    private NetworkSoundController nsc;
    private StatManager sm;

	// Use this for initialization
	void Start () {
        DOTween.Init();
		player = GetComponentInParent<playerClassAdd>().player;
		uiController = GetComponentInParent<UIController>();
        nsc = GetComponentInParent<NetworkSoundController>();
        sm = StatManager.instance;

        sugarPickupTime = GameManager.instance.sugarPickupTime;
		dropoffDelay = GameManager.instance.dropoffDelay;
	}

	// Update is called once per frame
	void Update () {

		if (!player.isStunned)
		{
			if (Input.GetKeyDown (KeyCode.Q)&& !dropping) {
					DropSugar ();
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "SugarCube")
		{
			if (player.sugarInBackpack < player.maxCanCarry) {
				//StartCoroutine(PickupSugarAni(other.gameObject));
				callback.CmdSugarPickup (other.gameObject);
			}
			return;
		}

		if (other.tag == "Dropoff Point")
		{
			runAnimation = true;
			if (player.dropoffPoint == other.gameObject) //If player owns this dropoff point
			{
				runAnimation = true;
                player.inBase = true;
                sm.UpdateStat(Stats.TimesVisitedBase);

				if (sugarInBackpack.Count > 0)
					StartCoroutine(DropoffSugarAni(other.gameObject));

				return;
			}
            else //If player does not own this dropoff point
			{
                PlayerClass otherPlayer = GameManager.instance.GetPlayerFromDropoff(other.gameObject);
                sm.UpdateStat(Stats.TimesVisitedOtherBases);

                if(!otherPlayer.inBase)
				    StartCoroutine(StealSugarAni(other.gameObject, otherPlayer));
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Dropoff Point")
		{
			runAnimation = false;
            uiController.CancelCrafting();
            player.inBase = false;
			if (player.craftingUIOpen)
				player.craftingUIOpen = false;
		}
	}

	public void SugarTransport(int cost) //Not being used, however still being called by ItemScript
	{
		if (sugarInBackpack.Count > 0)
		{
			if (cost == 0)
				cost = 1;

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
            nsc.CmdPlaySFX("SugarDrop", transform.parent.gameObject, 1f, true);
            callback.sugarCall ();
		}
	}

	public void StunDropSugar(int numToDrop = -1) //When stunned, drop sugar
	{
		if (sugarInBackpack.Count > 0)
		{
			int dropAmount;
            if (sugarInBackpack.Count == 1)
            {
                dropAmount = 1;
            }
            else
            {
                if (numToDrop < 0)
                    dropAmount = sugarInBackpack.Count / 2;
                else
                    dropAmount = numToDrop;
            }

			for (int x = 0; x < dropAmount; x++)
			{
                //StartCoroutine(DropSugarAni());
                nsc.CmdPlaySFX("SugarDrop", transform.parent.gameObject, 1f, true);
                callback.sugarCall ();
			}
		}
	}

	public IEnumerator DropSugarAni()
	{
		dropping = true;
		GameObject sugar = sugarInBackpack[0];
		sugar.SetActive(true);
		sugarInBackpack.Remove(sugarInBackpack[0]);
		sugar.transform.parent = null;
		float saveTime = Time.time;
		dropping = false;
		player.DropSugar();
		uiController.UpdateBackpackScore(player.sugarInBackpack);
		yield return null;
	}

	public IEnumerator DropSugarMovement(){
		GameObject sugar = sugarInBackpack[0];
		float saveTime = Time.time;
		Vector3 randomDropLoc = transform.parent.position + transform.forward * -15f;
		sugar.transform.DOMove (randomDropLoc, sugarPickupTime / 2);
		saveTime = Time.time;
		while (Time.time < saveTime + (sugarPickupTime / 2))
			yield return null;
		sugar.transform.position = randomDropLoc;
        sugar.GetComponent<sugarPickupable>().CheckForGround();
	}




	private IEnumerator StealSugarAni(GameObject dropoffPoint, PlayerClass otherPlayer)
	{
        //Steal sugar from enemy stash
		if (otherPlayer.currentPlayerScore > 0 && player.currentPlayerScore < player.maxCanCarry)
		{
            nsc.CmdPlaySFX("Sugar Pickup", transform.parent.gameObject, 1f, true);

            GameObject sugar = otherPlayer.dropoffPoint.transform.parent.GetChild(1).gameObject;
			sugarInBackpack.Add(sugar);
			otherPlayer.LoseSugar(1);
            player.PickupSugar();
            sm.UpdateStat(Stats.SugarStolen);

            uiController.UpdateBackpackScore(player.sugarInBackpack);
            otherPlayer.playerGO.GetComponent<UIController>().UpdateStashUI(otherPlayer.currentPlayerScore);

            Vector3 topPos = new Vector3(dropoffPoint.transform.position.x, dropoffPoint.transform.position.y + 2, dropoffPoint.transform.position.z);
			sugar.transform.parent = null;
			sugar.SetActive(true);
			Vector3 saveScale = sugar.transform.localScale;

            sugar.transform.DOScale(Vector3.zero, sugarPickupTime);
            sugar.transform.DOMove(transform.position, sugarPickupTime);
            float saveTime = Time.time;
            while (Time.time < saveTime + (sugarPickupTime / 2))
                yield return null;


            sugar.SetActive(false);
			sugar.transform.localScale = saveScale; //Reset sugar scale, might need to change later
			sugar.transform.parent = transform;
			sugar.transform.position = transform.position;

            if (otherPlayer.currentPlayerScore > 0 && runAnimation)
            {
                yield return new WaitForSeconds(2f);
                StartCoroutine(StealSugarAni(dropoffPoint, otherPlayer));
            }
		}
	}


	public void PickupSugarVoid(GameObject other){
		StartCoroutine(PickupSugarAni(other.gameObject));
	}

	public IEnumerator PickupSugarAni(GameObject sugar)
	{
        //Pick up sugar from the ground
        //sugar.GetComponent<SimpleRotate>().enabled = false;
        nsc.CmdPlaySFX("Sugar Pickup", transform.parent.gameObject, 1f, true);

		dropping = true;
		sugar.GetComponent<BoxCollider>().enabled = false;
        sugar.transform.parent = transform;
		Vector3 saveScale = sugar.transform.localScale;

        sugar.transform.DOScale(Vector3.zero, sugarPickupTime / 2);
        sugar.transform.DOMove(transform.position, sugarPickupTime / 2);
        float saveTime = Time.time;
        while (Time.time < saveTime + (sugarPickupTime / 2))
            yield return null;

        sugar.SetActive(false);
		sugar.transform.localScale = saveScale;
		dropping = false;
		player.PickupSugar();
		sugarInBackpack.Add(sugar);
		uiController.UpdateBackpackScore(player.sugarInBackpack);
        sm.UpdateStat(Stats.SugarCollected);
	}

	private IEnumerator DropoffSugarAni(GameObject dropoffPoint)
	{
        //Drop off sugar in the player's stash
        nsc.CmdPlaySFX("SugarDrop", transform.parent.gameObject, 1f, true);

        Vector3 saveScale = sugarInBackpack[0].transform.localScale;
		GameObject sugar = sugarInBackpack[0];
		sugarInBackpack.Remove(sugarInBackpack[0]);
		sugar.transform.parent = null;
		player.DropoffSugarInStash(); 

        uiController.UpdateBackpackScore(player.sugarInBackpack);
        uiController.UpdateStashUI(player.currentPlayerScore);

		sugar.SetActive(true);

		Vector3 topPos = new Vector3(sugar.transform.position.x, sugar.transform.position.y + 5, sugar.transform.position.z);

        sugar.transform.DOMove(topPos, sugarPickupTime / 2);
        float saveTime = Time.time;
        while (Time.time < saveTime + (sugarPickupTime / 2))
            yield return null;

        sugar.transform.DOMove(new Vector3(dropoffPoint.transform.position.x, dropoffPoint.transform.position.y, dropoffPoint.transform.position.z), sugarPickupTime / 2);
        sugar.transform.DOScale(Vector3.zero, sugarPickupTime / 2);
        saveTime = Time.time;
        while (Time.time < saveTime + (sugarPickupTime / 2))
            yield return null;

        sugar.SetActive(false);
		sugar.transform.localScale = saveScale;
		sugar.transform.parent = dropoffPoint.transform.parent;
		sugar.transform.position = new Vector3 (dropoffPoint.transform.position.x, dropoffPoint.transform.position.y + 50, dropoffPoint.transform.position.z);

		yield return new WaitForSeconds(dropoffDelay);

		if (sugarInBackpack.Count > 0 && runAnimation)
			StartCoroutine(DropoffSugarAni(dropoffPoint));
	}
}