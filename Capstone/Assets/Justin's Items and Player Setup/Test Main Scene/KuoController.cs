using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KuoController : MonoBehaviour {

    public float speed = 0.3f;
    public float fallThreshold = 3f;
    [HideInInspector]
    public PlayerClass player;
    public float bodySlamStunDur = 4f;
    public Collider bodySlamCol;
    private SphereCollider sc;
    private Rigidbody rb;
    public PlayerSugarPickup psp;

    [HideInInspector]
    public float timeToCraft = 3f;
    private float saveTTC;
    private int sugarNeededToCraft = 3;

    public Image CraftingBar;

	// Use this for initialization
	void Start () {
        sc = GetComponent<SphereCollider>();
        timeToCraft = CraftingController.instance.timeToCraft;
        saveTTC = timeToCraft;
        sugarNeededToCraft = CraftingController.instance.sugarNeededToCraft;
        rb = GetComponent<Rigidbody>();
        CraftingBar.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        float downVel = Mathf.Abs(rb.velocity.y);
        Debug.Log("downVel " + downVel);
        if(downVel > fallThreshold)
        {
            //Fall stun duration calculation
            float rbVelHalf = downVel / 2;
            float x = 1 + (downVel - fallThreshold);
            float stunDur = (rbVelHalf + (downVel / x)) - fallThreshold;
            Debug.Log(stunDur);
        }


        if (!player.isStunned)
        {
            //Movement Controls
            if (Input.GetKey(KeyCode.W))
                rb.velocity = new Vector3(rb.velocity.x, 0, speed);

            if (Input.GetKey(KeyCode.S))
                rb.velocity = new Vector3(rb.velocity.x, 0, -speed);

            if (Input.GetKey(KeyCode.A))
                rb.velocity = new Vector3(-speed, 0, rb.velocity.z);

            if (Input.GetKey(KeyCode.D))
                rb.velocity = new Vector3(speed, 0, rb.velocity.z);

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                rb.velocity = Vector3.zero;



            //Use Item
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!player.usingItem)
                {
                    if (player.item != null)
                        player.UseItem();
                    else
                        StartCoroutine(UseBodySlam());
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Dropoff Point")
        {
            //Craft Controls
            if (player.currentPlayerScore >= sugarNeededToCraft)
            {
                //Offensive
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    CraftItem(ItemType.Offensive);
                }
                //Defensive
                else if (Input.GetKey(KeyCode.Alpha2))
                {
                    CraftItem(ItemType.Defensive);
                }
                //Utility
                else if (Input.GetKey(KeyCode.Alpha3))
                {
                    CraftItem(ItemType.Utility);
                }

                if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Alpha3))
                {
                    StopCraftItem();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Dropoff Point")
            StopCraftItem();
    }

    public void CraftItem(ItemType type)
    {
        if(timeToCraft > 0)
        {
            CraftingBar.gameObject.SetActive(true);
            timeToCraft -= Time.deltaTime;
            CraftingBar.fillAmount = Mathf.InverseLerp(0, saveTTC, saveTTC - timeToCraft);
        } else
        {
            CraftingController.instance.EnableItem(type, player);
            player.LoseSugar(sugarNeededToCraft);
            timeToCraft = saveTTC;
            CraftingBar.gameObject.SetActive(false);
        }
    }

    public void StopCraftItem()
    {
        timeToCraft = saveTTC;
        CraftingBar.gameObject.SetActive(false);
    }

    public void StunPlayer(float duration)
    {
        StartCoroutine(StunPlayerCoroutine(duration));
    }

    private IEnumerator StunPlayerCoroutine(float duration)
    {
        if (!player.isInvulnerable)
        {
            Debug.Log(gameObject.name + " is stunned!");
            psp.StunDropSugar();
            player.isStunned = true;
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(duration);
            player.isStunned = false;
        }
    }

    private IEnumerator UseBodySlam()
    {
        Debug.Log("Use Body Slam");
        StartCoroutine(StunPlayerCoroutine(bodySlamStunDur / 2));
        player.usingItem = true;
        bodySlamCol.enabled = true;
        yield return new WaitForSeconds(0.25f);
        bodySlamCol.enabled = false;
        player.usingItem = false;
    }
}
