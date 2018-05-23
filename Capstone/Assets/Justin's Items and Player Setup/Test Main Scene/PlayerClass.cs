using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jkuo;

[System.Serializable]
public class PlayerClass {
    public int playerNum;
    public GameObject playerGO;
    public GameObject dropoffPoint;
    public bool inBase = false;
    public string playerName;
    public int maxCanCarry = 10;
    public int sugarInBackpack = 0;
    public int currentPlayerScore = 0;
    public bool isStunned = false;
    public bool isInvulnerable = false;
    public CraftingController.CraftableItem item;
    public int itemNum;
	public int itemCharges;
    public bool usingItem = false;
	public bool showCraftingUI = false;
    public bool craftingUIOpen = false;
	public GameObject currentItem;
	public string currentItemString;
    public bool playerPaused = false;
    public int outfitNum;
    

    private GameManager gm;
    public StatManager sm;
    private net_PlayerController npc;
    private float minSpeed;
    private float speedPerSugar;
    

    //Basically acts as the Start() function
    public void SetUpPlayer(int playerNum, int maxCanCarry, GameObject playerGO, GameObject dropoffPoint, string playerName)
    {
        this.playerNum = playerNum;
        this.maxCanCarry = maxCanCarry;
        this.playerGO = playerGO;
        this.dropoffPoint = dropoffPoint;
        this.playerName = playerName;

        gm = GameManager.instance;
        sm = StatManager.instance;
        npc = playerGO.GetComponent<net_PlayerController>();
        minSpeed = gm.minSpeed;
        speedPerSugar = gm.speedPerSugar;
    }

    public void PickupSugar()
    {
        sugarInBackpack++;

        if (npc.baseSpeed > minSpeed)
            npc.baseSpeed -= speedPerSugar;
    }

    public void DropoffSugarInStash()
    {
        currentPlayerScore++;
        sugarInBackpack--;
        npc.baseSpeed += speedPerSugar;

        if (currentPlayerScore >= gm.numSugarToWin)
            GameOverManager.instance.EndGame();
    }

    public void LoseSugar(int amount)
    {
        currentPlayerScore -= amount;
        npc.baseSpeed += speedPerSugar;
    }

    public void DropSugar()
    {
        sugarInBackpack--;
        npc.baseSpeed += speedPerSugar;
    }

    public void SetItem(PlayerClass player, CraftingController.CraftableItem item, int itemNum) //Deprecated
    {
        if (player.item != null)
        {
            CraftingController.instance.DisableItem(player.item, player.itemNum);
        }

        Debug.Log(player.playerName + " crafted a " + item.name + "!");
        player.item = item;
        player.itemNum = itemNum;
        item.gameObject.transform.position = player.playerGO.transform.position;
        item.gameObject.transform.parent = player.playerGO.transform;
        item.gameObject.SetActive(true);
    }

    public void CheckedRangedShield() //Deprecated
    {
        item.usesLeft--;
        if(item.usesLeft <= 0)
        {
            CraftingController.instance.DisableItem(item, itemNum);
            item = null;
            itemNum = 0;
        }
    }

    public void UseItem() //Deprecated
    {
        if(item.item == Item.Mirror || item.item == Item.Shield)
            return;

        ItemScript IS = item.gameObject.GetComponent<ItemScript>();
        item.usesLeft--;
        Debug.Log(item.name + " used 1 time. Number of uses left is " + item.usesLeft);
        IS.UseItem(item.type, item.item, this);
    }
}