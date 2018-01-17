using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemsScriptableObject : ScriptableObject {

    [System.Serializable]
    public class Items
    {
        public string name;
        public GameObject gameObject;
        public int numUses = 1;
        public float effectAmt = 1;
        public ItemType type;
        public Item item;
    }

    public Items[] OffensiveItems;
    public Items[] DefensiveItems;
    public Items[] UtilityItems;
}
