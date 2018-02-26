using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Offensive = 0,
    Defensive = 1,
    Utility = 2,
}

public enum Item
{
    Fan = 0,
    SugarPill = 1,
    Sword = 2,
    BaseBlocker = 3,
    Scythe = 4,
    MagicStaff = 5,
    SugarDusk = 6,
    Shield = 7,
    Mirror = 8,
    ChewedGum = 9,
    MouseTrap = 10,
    LegoWall = 11,
    SugarOrb = 12,
    Backpack = 13,
    Magnet = 14,
    RunningShoes = 15,
    SugarTransport = 16,
}

public class CraftingController : MonoBehaviour {

    public static CraftingController instance = null;
    public ItemsScriptableObject itemSO;

    public int sugarNeededToCraft = 3;
    public float timeToCraft = 3;

    public class CraftableItem
    {
        public string name;
        public GameObject gameObject;
        public int numUses = 1;
        [HideInInspector]
        public int usesLeft;
        public float effectAmt = 1;
        public ItemType type;
        public Item item;

        //[HideInInspector]
        public List<CraftableItem> inactiveItems = new List<CraftableItem>();
        //[HideInInspector]
        public List<CraftableItem> activeItems = new List<CraftableItem>();
    }

    //private List<CraftableItem> craftableItems = new List<CraftableItem>();

    [HideInInspector]
    public List<CraftableItem> OffensiveItems = new List<CraftableItem>();
    [HideInInspector]
    public List<CraftableItem> DefensiveItems = new List<CraftableItem>();
    [HideInInspector]
    public List<CraftableItem> UtilityItems = new List<CraftableItem>();
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        //GetItemsFromSOandPool();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnableItem(ItemType type, PlayerClass player)
    {
        int x = 0;
        switch (type)
        {
            case ItemType.Offensive:
                x = Random.Range(0, OffensiveItems.Count);

                if (OffensiveItems[x].inactiveItems.Count > 0)
                {
                    player.SetItem(player, OffensiveItems[x].inactiveItems[0], x);
                    OffensiveItems[x].activeItems.Add(OffensiveItems[x].inactiveItems[0]);
                    OffensiveItems[x].inactiveItems.Remove(OffensiveItems[x].inactiveItems[0]);
                } else
                {
                    EnableItem(type, player);
                }
                break;

            case ItemType.Defensive:
                x = Random.Range(0, DefensiveItems.Count);

                if (DefensiveItems[x].inactiveItems.Count > 0)
                {
                    if (DefensiveItems[x].item == Item.Mirror || DefensiveItems[x].item == Item.Shield)
                        DefensiveItems[x].gameObject.GetComponent<Collider>().enabled = true;
                    else
                        DefensiveItems[x].gameObject.GetComponent<Collider>().enabled = false;

                    player.SetItem(player, DefensiveItems[x].inactiveItems[0], x);
                    DefensiveItems[x].activeItems.Add(DefensiveItems[x].inactiveItems[0]);
                    DefensiveItems[x].inactiveItems.Remove(DefensiveItems[x].inactiveItems[0]);
                } else
                {
                    EnableItem(type, player);
                }
                break;

            case ItemType.Utility:
                x = Random.Range(0, UtilityItems.Count);

                if (UtilityItems[x].inactiveItems.Count > 0)
                {
                    player.SetItem(player, UtilityItems[x].inactiveItems[0], x);
                    UtilityItems[x].activeItems.Add(UtilityItems[x].inactiveItems[0]);
                    UtilityItems[x].inactiveItems.Remove(UtilityItems[x].inactiveItems[0]);
                } else
                {
                    EnableItem(type, player);
                }
                break;
        }
    }

    public void DisableItem(CraftableItem item, int itemNum)
    {
        item.gameObject.SetActive(false);

        if(item.item == Item.Mirror || item.item == Item.Shield)
        {
            item.gameObject.GetComponent<Collider>().enabled = false;
        }

        item.gameObject.transform.position = Vector3.zero;
        item.gameObject.transform.parent = transform;
        item.gameObject.GetComponent<Collider>().enabled = false;
        item.usesLeft = item.numUses;

        switch (item.type)
        {
            case ItemType.Offensive:
                OffensiveItems[itemNum].inactiveItems.Add(item);
                OffensiveItems[itemNum].activeItems.Remove(item);
                break;

            case ItemType.Defensive:
                DefensiveItems[itemNum].inactiveItems.Add(item);
                DefensiveItems[itemNum].activeItems.Remove(item);
                break;

            case ItemType.Utility:
                UtilityItems[itemNum].inactiveItems.Add(item);
                UtilityItems[itemNum].activeItems.Remove(item);
                break;
        }
    }

    private void GetItemsFromSOandPool()
    {
        //Get items from Scriptable Object
        foreach(ItemsScriptableObject.Items i in itemSO.OffensiveItems)
        {
            for(int x = 0; x < GameManager.instance.numPlayers * 2; x++)
            {
                GameObject item = Instantiate(i.gameObject, Vector3.zero, Quaternion.identity, transform);
                item.SetActive(false);
                CraftableItem citem = new CraftableItem();
                citem.name = i.name;
                citem.gameObject = item;
                citem.numUses = i.numUses;
                citem.usesLeft = i.numUses;
                citem.effectAmt = i.effectAmt;
                citem.type = i.type;
                citem.item = i.item;
                //craftableItems.Add(citem);
                citem.inactiveItems.Add(citem);
                OffensiveItems.Add(citem);
            }
        }

        foreach (ItemsScriptableObject.Items i in itemSO.DefensiveItems)
        {
            for (int x = 0; x < GameManager.instance.numPlayers * 2; x++)
            {
                GameObject item = Instantiate(i.gameObject, Vector3.zero, Quaternion.identity, transform);
                item.SetActive(false);
                CraftableItem citem = new CraftableItem();
                citem.name = i.name;
                citem.gameObject = item;
                citem.numUses = i.numUses;
                citem.usesLeft = i.numUses;
                citem.effectAmt = i.effectAmt;
                citem.type = i.type;
                citem.item = i.item;
                //craftableItems.Add(citem);
                citem.inactiveItems.Add(citem);
                DefensiveItems.Add(citem);
            }
        }

        foreach (ItemsScriptableObject.Items i in itemSO.UtilityItems)
        {
            for (int x = 0; x < GameManager.instance.numPlayers * 2; x++)
            {
                GameObject item = Instantiate(i.gameObject, Vector3.zero, Quaternion.identity, transform);
                item.SetActive(false);
                CraftableItem citem = new CraftableItem();
                citem.name = i.name;
                citem.gameObject = item;
                citem.numUses = i.numUses;
                citem.usesLeft = i.numUses;
                citem.effectAmt = i.effectAmt;
                citem.type = i.type;
                citem.item = i.item;
                //craftableItems.Add(citem);
                citem.inactiveItems.Add(citem);
                UtilityItems.Add(citem);
            }
        }
    }
}
