using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private float bodySlamDuration;
    private Collider itemCol;

    private Dictionary<string, System.Action> OffensiveItems = new Dictionary<string, System.Action>();
    private Dictionary<string, System.Action> DefensiveItems = new Dictionary<string, System.Action>();
    private Dictionary<string, System.Action> UtilityItems = new Dictionary<string, System.Action>();

    private PlayerClass currentPlayer;
    private CraftingController.CraftableItem currentItem;
    private int currentItemNum;
    private KuoController playerKC;

    void Awake()
    {
        // =========== IMPORTANT ===========
        //STRINGS MUST BE THE SAME AS THE ITEM NAME GIVEN IN THE CRAFTING CONTROLLER
        //SCRIPT WILL BREAK IF NOT CORRECT


        // =========== OFFENSIVE ITEMS ===========

        OffensiveItems.Add("Minigun", UseMinigun);
        OffensiveItems.Add("Sniper Rifle", UseSniperRifle);
        OffensiveItems.Add("Sword", UseSword);
        OffensiveItems.Add("Grenade", UseGrenade);
        OffensiveItems.Add("Scythe", UseScythe);
        OffensiveItems.Add("Magic Staff", UseMagicStaff);
        OffensiveItems.Add("Hand Cannon", UseHandCannon);
        OffensiveItems.Add("FTB Cannon", UseFTBCannon);
        OffensiveItems.Add("Fireball", UseFireball);
        OffensiveItems.Add("Frost Beam", UseFrostBeam);
        OffensiveItems.Add("Lightning", UseLightning);

        // =========== DEFENSIVE ITEMS ===========

        DefensiveItems.Add("Shield", UseShield);
        DefensiveItems.Add("Mirror", UseMirror);
        DefensiveItems.Add("Chewed Gum", UseChewedGum);
        DefensiveItems.Add("Mouse Trap", UsedMouseTrap);
        DefensiveItems.Add("Lego Wall", UseLegoWall);

        // =========== UTILITY ITEMS ===========

        UtilityItems.Add("Grappling Hook", UseGrapplingHook);
        UtilityItems.Add("Backpack", UseBackpack);
        UtilityItems.Add("Magnet", UseMagnet);
        UtilityItems.Add("Running Shoes", UseRunningShoes);
        UtilityItems.Add("Breezy Jardons", UseBreezyJardons);

    }

    void Start()
    {
        itemCol = GetComponent<Collider>();
        itemCol.enabled = false;
        bodySlamDuration = transform.parent.GetComponent<KuoController>().bodySlamStunDur;
    }

	public void UseItem(ItemType type, string name, PlayerClass player)
    {
        currentPlayer = player;
        currentItem = currentPlayer.item;
        currentItemNum = currentPlayer.itemNum;
        switch (type)
        {
            case ItemType.Offensive:
                OffensiveItems[name].Invoke();
                break;

            case ItemType.Defensive:
                DefensiveItems[name].Invoke();
                break;

            case ItemType.Utility:
                UtilityItems[name].Invoke();
                break;
        }
    }


    // =========== OFFENSIVE ITEMS ===========

    public void UseMinigun()
    {
        Debug.Log("Used Minigun");
    }

    public void UseSniperRifle()
    {
        Debug.Log("Used Sniper Rifle");
    }

    public void UseSword()
    {
        Debug.Log("Used Sword");
    }

    public void UseGrenade()
    {
        Debug.Log("Used Grenade");
    }

    public void UseScythe()
    {
        Debug.Log("Used Scythe");
    }

    public void UseMagicStaff()
    {
        Debug.Log("Used Magic Staff");
    }

    public void UseHandCannon()
    {
        Debug.Log("Used Hand Cannon");
    }

    public void UseFTBCannon()
    {
        Debug.Log("Used FTB Cannon");
    }

    public void UseFireball()
    {
        Debug.Log("Used Fireball");
    }

    public void UseFrostBeam()
    {
        Debug.Log("Used Frost Beam");
    }

    public void UseLightning()
    {
        Debug.Log("Used Lightning");
    }



    // =========== DEFENSIVE ITEMS ===========

    public void UseShield()
    {
        Debug.Log("Used Shield");
    }

    public void UseMirror()
    {
        Debug.Log("Used Mirror");
    }

    public void UseChewedGum()
    {
        Debug.Log("Used Chewed Gum");
    }

    public void UsedMouseTrap()
    {
        Debug.Log("Used Mouse Trap");
    }

    public void UseLegoWall()
    {
        Debug.Log("Used Lego Wall");
        StartCoroutine(LegoWallCoroutine());
    }

    private IEnumerator LegoWallCoroutine()
    {
        currentPlayer.usingItem = true;
        itemCol.enabled = true;
        GameObject wall = currentPlayer.item.gameObject;
        wall.transform.parent = null;
        wall.transform.position = new Vector3(currentPlayer.playerGO.transform.position.x, currentPlayer.playerGO.transform.position.y - 2, currentPlayer.playerGO.transform.position.z + 2);

        while (wall.transform.position.y < currentPlayer.playerGO.transform.position.y)
        {
            wall.transform.Translate(Vector3.up * 0.1f);
            yield return null;
        }

        currentPlayer.usingItem = false;
        currentPlayer.item = null;
        currentPlayer.itemNum = 0;

        yield return new WaitForSeconds(currentItem.effectAmt);

        while (wall.transform.position.y > wall.transform.position.y - 2)
        {
            wall.transform.Translate(Vector3.down * 0.1f);
            yield return null;
        }

        itemCol.enabled = false;
        CheckItemUses(false);
    }


    // =========== UTILITY ITEMS ===========

    public void UseGrapplingHook()
    {
        Debug.Log("Used Grappling Hook");
    }

    public void UseBackpack()
    {
        Debug.Log("Used Backpack");
        currentPlayer.maxCanCarry += (int) currentPlayer.item.effectAmt;

        CheckItemUses();
    }

    public void UseMagnet()
    {
        Debug.Log("Used Magnet");
        StartCoroutine(UseMagnetCoroutine());
    }

    private IEnumerator UseMagnetCoroutine()
    {
        currentPlayer.usingItem = true;
        Transform pickUpGO = currentPlayer.playerGO.transform.GetChild(0);
        pickUpGO.GetComponent<SphereCollider>().radius += currentItem.effectAmt;
        yield return new WaitForSeconds(5f);
        pickUpGO.GetComponent<SphereCollider>().radius -= currentItem.effectAmt;
        currentPlayer.usingItem = false;

        CheckItemUses();
    }

    public void UseRunningShoes()
    {
        Debug.Log("Used Running Shoes");
    }

    public void UseBreezyJardons()
    {
        Debug.Log("Used Breezy Jardon's");
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Stun player
            if (currentPlayer.item != null)
                other.gameObject.GetComponent<KuoController>().StunPlayer(currentPlayer.item.effectAmt);
            else
                other.gameObject.GetComponent<KuoController>().StunPlayer(bodySlamDuration);
        }
    }



    private void CheckItemUses(bool setItemNull = true)
    {
        if (currentPlayer.item.usesLeft <= 0)
        {
            CraftingController.instance.DisableItem(currentItem, currentItemNum);

            if (!setItemNull)
            {
                currentPlayer.item = null;
                currentPlayer.itemNum = 0;
            }
        }
    }
}
