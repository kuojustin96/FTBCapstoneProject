using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSugarPickup : MonoBehaviour {

    private PlayerClass player;
    private float sugarPickupSpeed;
    private float dropoffDelay;
    private List<GameObject> sugarInBackpack = new List<GameObject>();
    private bool runDropoffAni = false;

	// Use this for initialization
	void Start () {
        player = GetComponentInParent<KuoController>().player;
        sugarPickupSpeed = GameManager.instance.sugarPickUpSpeed;
        dropoffDelay = GameManager.instance.dropoffDelay;
	}
	
	// Update is called once per frame
	void Update () {
		
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
            if (player.dropoffPoint == other.gameObject)
            {
                //Debug.Log(sugarInBackpack.Count);
                if (sugarInBackpack.Count > 0)
                {
                    runDropoffAni = true;
                    StartCoroutine(DropoffSugarAni(other.gameObject));
                }
                return;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Dropoff Point")
        {
            if (player.dropoffPoint == other.gameObject)
            {
                runDropoffAni = false;
                return;
            }
        }
    }

    private IEnumerator PickupSugarAni(GameObject sugar)
    {
        sugar.GetComponent<SimpleRotate>().enabled = false;
        sugar.GetComponent<BoxCollider>().enabled = false;
        player.PickupSugar();
        sugarInBackpack.Add(sugar);
        Vector3 saveScale = sugar.transform.localScale;

        while (sugar.transform.position != transform.position)
        {
            sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, transform.position, sugarPickupSpeed);
            yield return null;
        }

        sugar.SetActive(false);
        sugar.transform.localScale = saveScale;
        sugar.transform.parent = transform;
    }

    private IEnumerator DropoffSugarAni(GameObject dropoffPoint)
    {
        int count = 0;

        Vector3 saveScale = sugarInBackpack[0].transform.localScale;
        GameObject sugar = sugarInBackpack[0];
        sugarInBackpack.Remove(sugarInBackpack[0]);
        sugar.transform.parent = null;
        player.DropoffSugar();
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

        yield return new WaitForSeconds(dropoffDelay);

        if (sugarInBackpack.Count > 0 && runDropoffAni)
            StartCoroutine(DropoffSugarAni(dropoffPoint));
     }
}