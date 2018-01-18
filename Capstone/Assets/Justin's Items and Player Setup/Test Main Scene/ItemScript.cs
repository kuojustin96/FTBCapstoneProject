using System.Collections;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private float bodySlamDuration;
    private Collider itemCol;

    //private Dictionary<string, System.Action> OffensiveItems = new Dictionary<string, System.Action>();
    //private Dictionary<string, System.Action> DefensiveItems = new Dictionary<string, System.Action>();
    //private Dictionary<string, System.Action> UtilityItems = new Dictionary<string, System.Action>();

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

        //OffensiveItems.Add("Minigun", UseMinigun);
        //OffensiveItems.Add("Sniper Rifle", UseSniperRifle);
        //OffensiveItems.Add("Sword", UseSword);
        //OffensiveItems.Add("Grenade", UseGrenade);
        //OffensiveItems.Add("Scythe", UseScythe);
        //OffensiveItems.Add("Magic Staff", UseMagicStaff);
        //OffensiveItems.Add("Hand Cannon", UseHandCannon);
        ////OffensiveItems.Add("FTB Cannon", UseFTBCannon);
        ////OffensiveItems.Add("Fireball", UseFireball);
        ////OffensiveItems.Add("Frost Beam", UseFrostBeam);
        ////OffensiveItems.Add("Lightning", UseLightning);

        //// =========== DEFENSIVE ITEMS ===========

        //DefensiveItems.Add("Shield", UseShield);
        //DefensiveItems.Add("Mirror", UseMirror);
        //DefensiveItems.Add("Chewed Gum", UseChewedGum);
        //DefensiveItems.Add("Mouse Trap", UsedMouseTrap);
        //DefensiveItems.Add("Lego Wall", UseLegoWall);

        //// =========== UTILITY ITEMS ===========

        //UtilityItems.Add("Grappling Hook", UseGrapplingHook);
        //UtilityItems.Add("Backpack", UseBackpack);
        //UtilityItems.Add("Magnet", UseMagnet);
        //UtilityItems.Add("Running Shoes", UseRunningShoes);
        //UtilityItems.Add("Breezy Jardons", UseBreezyJardons);

    }

    void Start()
    {
        itemCol = GetComponent<Collider>();
        itemCol.enabled = false;
        bodySlamDuration = transform.parent.GetComponent<KuoController>().bodySlamStunDur;
    }

	public void UseItem(ItemType type, Item item, PlayerClass player)
    {
        currentPlayer = player;
        currentItem = currentPlayer.item;
        currentItemNum = currentPlayer.itemNum;
        switch (type)
        {
            case ItemType.Offensive:
                //OffensiveItems[name].Invoke();
                switch (item)
                {
                    case Item.Minigun:
                        Debug.Log("Used Minigun");
                        break;

                    case Item.SniperRifle:
                        Debug.Log("Used Sniper Rifle");
                        break;

                    case Item.Sword:
                        Debug.Log("Used Sword");
                        StartCoroutine(UseSword());
                        break;

                    case Item.Grenade:
                        Debug.Log("Used Grenade");
                        break;

                    case Item.Scythe:
                        Debug.Log("Used Scythe");
                        break;

                    case Item.MagicStaff:
                        Debug.Log("Used Magic Staff");
                        break;

                    case Item.HandCannon:
                        Debug.Log("Used Hand Cannon");
                        break;
                }
                break;

            case ItemType.Defensive:
                //DefensiveItems[name].Invoke();
                switch (item)
                {
                    case Item.Shield:
                        Debug.Log("Used Shield");
                        break;

                    case Item.Mirror:
                        Debug.Log("Used Mirror");
                        break;

                    case Item.ChewedGum:
                        Debug.Log("Used Chewed Gum");
                        StartCoroutine(UseChewedGum());
                        break;

                    case Item.MouseTrap:
                        Debug.Log("Used Mouse Trap");
                        break;

                    case Item.LegoWall:
                        Debug.Log("Used Lego Wall");
                        StartCoroutine(LegoWall());
                        break;
                }
                break;

            case ItemType.Utility:
                //UtilityItems[name].Invoke();
                switch (item)
                {
                    case Item.GrapplingHook:
                        Debug.Log("Used Grappling Hook");
                        break;

                    case Item.Backpack:
                        Debug.Log("Used Backpack");
                        currentPlayer.maxCanCarry += (int)currentPlayer.item.effectAmt;
                        CheckItemUses();
                        break;

                    case Item.Magnet:
                        Debug.Log("Used Magnet");
                        StartCoroutine(UseMagnet());
                        break;

                    case Item.RunningShoes:
                        Debug.Log("Used Running Shoes");
                        break;

                    case Item.BreezyJardons:
                        Debug.Log("Used Breezy Jardon's");
                        break;
                }
                break;
        }
    }

    #region Offensive Items
    // =========== OFFENSIVE ITEMS ===========

    public void UseMinigun()
    {
        Debug.Log("Used Minigun");
    }

    public void UseSniperRifle()
    {
        Debug.Log("Used Sniper Rifle");
    }

    private IEnumerator UseSword()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z);
        itemCol.enabled = true;
        yield return new WaitForSeconds(0.5f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - 90, transform.eulerAngles.y, transform.eulerAngles.z);
        itemCol.enabled = false;
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
    #endregion

    #region Defensive Items
    // =========== DEFENSIVE ITEMS ===========

    public void UseShield()
    {
        Debug.Log("Used Shield");
    }

    public void UseMirror()
    {
        Debug.Log("Used Mirror");
    }

    public IEnumerator UseChewedGum()
    {
        Debug.Log("Used Chewed Gum");
        GameObject chewingGum = currentItem.gameObject;
        GameObject player = currentPlayer.playerGO;
        chewingGum.transform.parent = null;
        chewingGum.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - player.transform.localScale.y / 2, player.transform.position.z - 3);

        itemCol.enabled = true;
        yield return new WaitForSeconds(8f);
        itemCol.enabled = false;
        CheckItemUses(false);
    }

    public void UsedMouseTrap()
    {
        Debug.Log("Used Mouse Trap");
    }

    private IEnumerator LegoWall()
    {
        currentPlayer.usingItem = true;
        itemCol.enabled = true;
        GameObject wall = currentItem.gameObject;
        wall.transform.parent = null;
        wall.transform.position = new Vector3(currentPlayer.playerGO.transform.position.x, currentPlayer.playerGO.transform.position.y - 2, currentPlayer.playerGO.transform.position.z + 2);
        Vector3 saveWallPos = wall.transform.position;
        currentPlayer.usingItem = false;

        while (wall.transform.position.y < currentPlayer.playerGO.transform.position.y)
        {
            wall.transform.Translate(Vector3.up * 0.2f);
            yield return null;
        }


        currentPlayer.item = null;
        currentPlayer.itemNum = 0;

        yield return new WaitForSeconds(currentItem.effectAmt);

        while (wall.transform.position.y > saveWallPos.y)
        {
            wall.transform.Translate(Vector3.down * 0.2f);
            yield return null;
        }

        itemCol.enabled = false;
        CheckItemUses(false);
    }
    #endregion

    #region Utility Items
    // =========== UTILITY ITEMS ===========

    public void UseGrapplingHook()
    {
        Debug.Log("Used Grappling Hook");
    }

    private IEnumerator UseMagnet()
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
    #endregion

    //-------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject != currentPlayer.playerGO)
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
        if (currentItem.usesLeft <= 0)
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
