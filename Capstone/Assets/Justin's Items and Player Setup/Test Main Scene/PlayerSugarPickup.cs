using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSugarPickup : MonoBehaviour {

    private PlayerClass player;
    private float sugarPickupSpeed;
    private float dropoffDelay;
    private List<GameObject> sugarInBackpack = new List<GameObject>();
    private bool runAnimation = false;

	// Use this for initialization
	void Start () {
        player = GetComponentInParent<KuoController>().player;
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
            runAnimation = true;
            if (player.dropoffPoint == other.gameObject) //If player owns this dropoff point
            {
                //Debug.Log(sugarInBackpack.Count);
                if (sugarInBackpack.Count > 0)
                {
                    //runAnimation = true;
                    StartCoroutine(DropoffSugarAni(other.gameObject));
                }
                return;
            } else //If player does not own this dropoff point
            {
                StartCoroutine(StealSugarAni(other.gameObject));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Dropoff Point")
        {
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
            GameObject dropoffPoint = GameManager.instance.DropoffPoints[player.playerNum];
            for (int x = 0; x < cost; x++)
            {
                sugarInBackpack[0].transform.parent = null;
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
        player.DropSugar();
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

        Vector3 randomDropLoc = Random.onUnitSphere * 1.5f;
        randomDropLoc.y = 1f;
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
        int count = 0;

        PlayerClass otherPlayer = GameManager.instance.GetPlayerFromDropoff(dropoffPoint);

        if (otherPlayer.currentPlayerScore > 0)
        {
            GameObject sugar = otherPlayer.dropoffPoint.transform.GetChild(0).gameObject;
            sugarInBackpack.Add(sugar);
            otherPlayer.LoseSugar(1);
            player.PickupSugar();
            Vector3 topPos = new Vector3(dropoffPoint.transform.position.x, dropoffPoint.transform.position.y + 2, dropoffPoint.transform.position.z);
            sugar.transform.parent = null;
            sugar.SetActive(true);

            while (count < 20)
            {
                count++;
                sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
                yield return null;
            }

            count = 0;

            while (count < 20)
            {
                count++;
                sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
                sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, transform.position, sugarPickupSpeed);
                yield return null;
            }

            sugar.SetActive(false);
            sugar.transform.localScale = Vector3.one; //Reset sugar scale, might need to change later
            sugar.transform.parent = transform;

            if (otherPlayer.currentPlayerScore > 0 && runAnimation)
                StartCoroutine(StealSugarAni(dropoffPoint));
        }
    }

    private IEnumerator PickupSugarAni(GameObject sugar)
    {
        sugar.GetComponent<SimpleRotate>().enabled = false;
        sugar.GetComponent<BoxCollider>().enabled = false;
        player.PickupSugar();
        sugarInBackpack.Add(sugar);
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
        int count = 0;

        Vector3 saveScale = sugarInBackpack[0].transform.localScale;
        GameObject sugar = sugarInBackpack[0];
        sugarInBackpack.Remove(sugarInBackpack[0]);
        sugar.transform.parent = null;
        player.DropoffSugarInStash();
        sugar.SetActive(true);

        Vector3 topPos = new Vector3(sugar.transform.position.x, sugar.transform.position.y + 1, sugar.transform.position.z);

        while(count < 10)
        {
            count++;
            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
            yield return null;
        }

        count = 0;

        while (count < 10)
        {
            count++;
            sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, dropoffPoint.transform.position, sugarPickupSpeed);
            yield return null;
        }

        sugar.SetActive(false);
        sugar.transform.localScale = saveScale;
        sugar.transform.parent = dropoffPoint.transform;

        yield return new WaitForSeconds(dropoffDelay);

        if (sugarInBackpack.Count > 0 && runAnimation)
            StartCoroutine(DropoffSugarAni(dropoffPoint));
     }
}