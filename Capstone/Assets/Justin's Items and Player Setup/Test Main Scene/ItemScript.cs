using System.Collections;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private float bodySlamDuration;
    private Collider itemCol;

    private PlayerClass currentPlayer;
    private CraftingController.CraftableItem currentItem;
    private int currentItemNum;
    private KuoController playerKC;

    void Start()
    {
        itemCol = GetComponent<Collider>();
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
                switch (item)
                {
                    case Item.Fan:
                        Debug.Log("Used Minigun");
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
                        UseMagicStaff();
                        break;

                    case Item.Shotgun:
                        Debug.Log("Used Hand Cannon");
                        break;
                }
                break;

            case ItemType.Defensive:
                switch (item)
                {
                    case Item.Shield:
                        //Debug.Log("Used Shield");
                        break;

                    case Item.Mirror:
                        //Debug.Log("Used Mirror");
                        break;

                    case Item.ChewedGum:
                        Debug.Log("Used Chewed Gum");
                        StartCoroutine(UseChewedGum());
                        break;

                    case Item.MouseTrap:
                        Debug.Log("Used Mouse Trap");
                        UsedMouseTrap();
                        break;

                    case Item.LegoWall:
                        Debug.Log("Used Lego Wall");
                        StartCoroutine(LegoWall());
                        break;
                }
                break;

            case ItemType.Utility:
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

    public void UseNerfGun()
    {
        Debug.Log("Used Nerf Gun");
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
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.forward, out hit, 5f))
        {
            if(hit.collider.tag == "Player")
            {
                KuoController kc = hit.collider.GetComponent<KuoController>();
                if (kc.player.item != null)
                {
                    if (kc.player.item.item != Item.Mirror)
                        kc.StunPlayer(currentItem.effectAmt);
                    else
                        kc.player.CheckedRangedShield();
                } else
                {
                    kc.StunPlayer(currentItem.effectAmt);
                }
            }
        }
    }

    public void UseShotgun()
    {
        Debug.Log("Used Shotgun");
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
        yield return new WaitForSeconds(8f); //Length item is active for
        itemCol.enabled = false;
        CheckItemUses(false);
    }

    public void UsedMouseTrap()
    {
        Debug.Log("Used Mouse Trap");
        GameObject chewingGum = currentItem.gameObject;
        GameObject player = currentPlayer.playerGO;
        chewingGum.transform.parent = null;
        chewingGum.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - player.transform.localScale.y / 2, player.transform.position.z - 3);
        itemCol.enabled = true;
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
        if(currentPlayer == null)
        {
            return;
        }

        if (other.gameObject != currentPlayer.playerGO)
        {
            if(currentItem.item == Item.Sword && other.gameObject.tag == "MeleeDefensiveShield")
            {
                currentItem.usesLeft--;
                CheckItemUses();
                return;
            }

            //if ((currentItem.item == Item.NerfGun || currentItem.item == Item.Shotgun) && other.gameObject.tag == "RangedDefensiveShield")
            //{
            //    currentItem.usesLeft--;
            //    CheckItemUses();
            //    return;
            //}

            if (other.gameObject.tag == "Player")
            {
                //Stun player
                if (currentPlayer.item != null)
                    other.gameObject.GetComponent<KuoController>().StunPlayer(currentPlayer.item.effectAmt);
                else
                    other.gameObject.GetComponent<KuoController>().StunPlayer(bodySlamDuration);

                if (currentItem.item == Item.MouseTrap)
                {
                    CheckItemUses(false);
                    itemCol.enabled = false;
                }
            }
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
