using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Offensive = 0,
    Defensive = 1,
    Utility = 2,
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

        [HideInInspector]
        public List<CraftableItem> inactiveItems = new List<CraftableItem>();
        [HideInInspector]
        public List<CraftableItem> activeItems = new List<CraftableItem>();
    }

    private List<CraftableItem> craftableItems = new List<CraftableItem>();

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

        GetItemsFromSOandPool();
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
        item.gameObject.transform.position = Vector3.zero;
        item.gameObject.transform.parent = transform;
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
            CraftableItem citem = new CraftableItem();
            citem.name = i.name;
            citem.gameObject = i.gameObject;
            citem.numUses = i.numUses;
            citem.effectAmt = i.effectAmt;
            citem.type = i.type;
            craftableItems.Add(citem);
        }

        foreach (ItemsScriptableObject.Items i in itemSO.DefensiveItems)
        {
            CraftableItem citem = new CraftableItem();
            citem.name = i.name;
            citem.gameObject = i.gameObject;
            citem.numUses = i.numUses;
            citem.effectAmt = i.effectAmt;
            citem.type = i.type;
            craftableItems.Add(citem);
        }

        foreach (ItemsScriptableObject.Items i in itemSO.UtilityItems)
        {
            CraftableItem citem = new CraftableItem();
            citem.name = i.name;
            citem.gameObject = i.gameObject;
            citem.numUses = i.numUses;
            citem.effectAmt = i.effectAmt;
            citem.type = i.type;
            craftableItems.Add(citem);
        }


        //Pool Items
        foreach (CraftableItem c in craftableItems)
        {
            for (int x = 0; x < GameManager.instance.numPlayers; x++)
            {
                GameObject item = Instantiate(c.gameObject, Vector3.zero, Quaternion.identity, transform);
                item.SetActive(false);
                c.gameObject = item;
                c.usesLeft = c.numUses;
                c.inactiveItems.Add(c);

                switch (c.type)
                {
                    case ItemType.Offensive:
                        OffensiveItems.Add(c);
                        break;

                    case ItemType.Defensive:
                        DefensiveItems.Add(c);
                        break;

                    case ItemType.Utility:
                        UtilityItems.Add(c);
                        break;
                }
            }
        }
    }
}
