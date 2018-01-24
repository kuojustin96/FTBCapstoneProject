using System.Collections;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private float bodySlamDuration;
    private Collider itemCol;

    private PlayerClass currentPlayer;
    private CraftingController.CraftableItem currentItem;
    private int currentItemNum;
    private KuoController playerKC;

    private Rigidbody rb;
    private bool waitingForHit;
    private Vector3 contactPoint;

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
                        Debug.Log("Used Fan");
                        UseFan();
                        break;

                    case Item.Sword:
                        Debug.Log("Used Sword");
                        StartCoroutine(UseSword());
                        break;

                    case Item.Scythe:
                        Debug.Log("Used Scythe");
                        break;

                    case Item.MagicStaff:
                        Debug.Log("Used Magic Staff");
                        UseMagicStaff();
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
                    case Item.SugarOrb:
                        Debug.Log("Used Sugar Orb");
                        StartCoroutine(UseSugarOrb());
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

                    case Item.SugarTransport:
                        Debug.Log("Used Sugar Transport");
                        StartCoroutine(UseSugarTransport());
                        break;

                    case Item.SugarDusk:
                        Debug.Log("Used Sugar Dusk");
                        StartCoroutine(UseSugarDusk());
                        break;

                    case Item.SugarPill:
                        Debug.Log("Used Sugar Pill");
                        StartCoroutine(UseSugarPill()); //Needs testing
                        break;

                    case Item.BaseBlocker:
                        Debug.Log("Used Base Blocker");
                        StartCoroutine(UseBaseBlocker());
                        break;
                }
                break;
        }
    }

    #region Offensive Items
    // =========== OFFENSIVE ITEMS ===========

    public void UseFan()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 2, Vector3.forward, Quaternion.identity, 10f);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Player" && hit.collider.gameObject != transform.parent.gameObject)
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.parent.forward - hit.collider.transform.eulerAngles) * currentItem.effectAmt, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator UseSword()
    {
        currentPlayer.usingItem = true;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z);
        itemCol.enabled = true;
        yield return new WaitForSeconds(0.5f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - 90, transform.eulerAngles.y, transform.eulerAngles.z);
        itemCol.enabled = false;

        currentPlayer.usingItem = false;
    }

    public void UseScythe()
    {
        Debug.Log("Used Scythe");
    }

    public void UseMagicStaff()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.forward, out hit, 20f))
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

    public IEnumerator UseSugarOrb()
    {
        currentPlayer.usingItem = true;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.AddForce(Vector3.forward * currentItem.effectAmt, ForceMode.Impulse);

        yield return null;
        Collider c = GetComponent<Collider>();
        c.enabled = true;

        waitingForHit = true;
        while (waitingForHit)
        {
            yield return null;
        }

        currentPlayer.playerGO.transform.position = contactPoint;
        CheckItemUses();
        currentPlayer.usingItem = false;

        c.enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
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

    private void UseRunningShoes()
    {
        Debug.Log("Used Running Shoes");
    }

    private IEnumerator UseSugarTransport()
    {
        currentPlayer.usingItem = true;

        float timeToWait = 5f;
        float saveTTW = timeToWait;
        KuoController kc = currentPlayer.playerGO.GetComponent<KuoController>();
        kc.CraftingBar.fillAmount = 0;
        kc.CraftingBar.gameObject.SetActive(true);
        while (timeToWait > 0 && !currentPlayer.isStunned && currentPlayer.playerGO.GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            timeToWait -= Time.deltaTime;
            kc.CraftingBar.fillAmount = Mathf.InverseLerp(0, saveTTW, saveTTW - timeToWait);
            yield return null;
        }

        if(timeToWait <= 0)
        {
            int cost = currentPlayer.sugarInBackpack / (int) currentItem.effectAmt;
            kc.psp.SugarTransport(cost);
        }

        kc.CraftingBar.gameObject.SetActive(false);
        CheckItemUses();

        currentPlayer.usingItem = false;
    }

    private IEnumerator UseSugarDusk()
    {
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(true);
        child.GetComponent<ParticleSystem>().Play();
        currentPlayer.playerGO.layer = 8;
        gameObject.layer = 8;
        yield return new WaitForSeconds(currentItem.effectAmt);
        CheckItemUses();
        transform.GetChild(0).gameObject.SetActive(false);
        currentPlayer.playerGO.layer = 0;
        gameObject.layer = 0;
    }

    private IEnumerator UseSugarPill()
    {
        currentPlayer.isInvulnerable = true;
        yield return new WaitForSeconds(currentItem.effectAmt);
        currentPlayer.isInvulnerable = false;
        playerKC.StunPlayer(currentItem.effectAmt / 4);
        CheckItemUses();
    }

    public IEnumerator UseBaseBlocker()
    {
        currentPlayer.usingItem = true;

        GameObject baseBlocker = currentItem.gameObject;
        baseBlocker.transform.parent = null;
        baseBlocker.transform.position = GameManager.instance.DropoffPoints[currentPlayer.playerNum].transform.position;

        foreach (Transform t in baseBlocker.transform)
            t.gameObject.SetActive(true);

        yield return new WaitForSeconds(currentItem.effectAmt);

        foreach (Transform t in baseBlocker.transform)
            t.gameObject.SetActive(false);

        CheckItemUses(false);

        currentPlayer.usingItem = false;
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            waitingForHit = false;
            contactPoint = collision.contacts[0].point;
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
