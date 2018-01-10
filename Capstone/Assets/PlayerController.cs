using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 0.3f;
    public PlayerClass player;
    private SphereCollider sc;

    [HideInInspector]
    public float timeToCraft = 3f;
    private float saveTTC;
    private int sugarNeededToCraft = 3;

	// Use this for initialization
	void Start () {
        sc = GetComponent<SphereCollider>();
        timeToCraft = CraftingController.instance.timeToCraft;
        saveTTC = timeToCraft;
        sugarNeededToCraft = CraftingController.instance.sugarNeededToCraft;
	}
	
	// Update is called once per frame
	void Update () {
        //Movement Controls
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed);
        }



        //Use Item
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(player.item != null)
            {
                player.UseItem();
            }
        }





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

    public void CraftItem(ItemType type)
    {
        if(timeToCraft > 0)
        {
            timeToCraft -= Time.deltaTime;
        } else
        {
            CraftingController.instance.EnableItem(type, player);
            player.LoseSugar(sugarNeededToCraft);
            timeToCraft = saveTTC;
        }
    }

    public void StopCraftItem()
    {
        timeToCraft = saveTTC;
    }
}
