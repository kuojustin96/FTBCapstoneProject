using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerClass {
    public int playerNum;
    public GameObject playerGO;
    public GameObject dropoffPoint;
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

    private UIController uiController;

    public void SetUpPlayer(int playerNum, int maxCanCarry, GameObject playerGO, GameObject dropoffPoint, string playerName)
    {
        this.playerNum = playerNum;
        this.maxCanCarry = maxCanCarry;
        this.playerGO = playerGO;
        this.dropoffPoint = dropoffPoint;
        this.playerName = playerName;
    }

    public void SetUIController(UIController uiController)
    {
        if (this.uiController == null)
        {
            this.uiController = uiController;
            uiController.UpdateMaxBackpackScore(maxCanCarry);
        }
    }

    public void PickupSugar()
    {
        sugarInBackpack++;
        uiController.UpdateBackpackScore(sugarInBackpack);
//        ScoreController.instance.UpdateBackpackScore(playerNum, sugarInBackpack);
    }

    public void DropoffSugarInStash()
    {
        currentPlayerScore++;
        sugarInBackpack--;
        uiController.UpdateBackpackScore(sugarInBackpack);
        //        ScoreController.instance.UpdateBackpackScore(playerNum, sugarInBackpack);
        //        ScoreController.instance.UpdateScore(playerNum, currentPlayerScore);
    }

    public void LoseSugar(int amount)
    {
        currentPlayerScore -= amount;
        //      ScoreController.instance.UpdateScore(playerNum, currentPlayerScore);
    }

    public void DropSugar()
    {
        sugarInBackpack--;
        uiController.UpdateBackpackScore(sugarInBackpack);
        //     ScoreController.instance.UpdateBackpackScore(playerNum, sugarInBackpack);
    }

    public void SetItem(PlayerClass player, CraftingController.CraftableItem item, int itemNum)
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

    public void CheckedRangedShield()
    {
        item.usesLeft--;
        if(item.usesLeft <= 0)
        {
            CraftingController.instance.DisableItem(item, itemNum);
            item = null;
            itemNum = 0;
        }
    }

    public void UseItem()
    {
        if(item.item == Item.Mirror || item.item == Item.Shield)
            return;

        ItemScript IS = item.gameObject.GetComponent<ItemScript>();
        item.usesLeft--;
        Debug.Log(item.name + " used 1 time. Number of uses left is " + item.usesLeft);
        IS.UseItem(item.type, item.item, this);
    }
}