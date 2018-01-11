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
    public CraftingController.CraftableItem item;
    public int itemNum;

    public void SetUpPlayer(int playerNum, int maxCanCarry, GameObject playerGO, GameObject dropoffPoint, string playerName)
    {
        this.playerNum = playerNum;
        this.maxCanCarry = maxCanCarry;
        this.playerGO = playerGO;
        this.dropoffPoint = dropoffPoint;
        this.playerName = playerName;
    }

    public void PickupSugar()
    {
        sugarInBackpack++;
        ScoreController.instance.UpdateBackpackScore(playerNum, sugarInBackpack);
    }

    public void DropoffSugar()
    {
        currentPlayerScore++;
        sugarInBackpack--;
        ScoreController.instance.UpdateBackpackScore(playerNum, sugarInBackpack);
        ScoreController.instance.UpdateScore(playerNum, currentPlayerScore);
    }

    public void LoseSugar(int amount)
    {
        currentPlayerScore -= amount;
        ScoreController.instance.UpdateScore(playerNum, currentPlayerScore);
    }

    public void SetItem(PlayerClass player, CraftingController.CraftableItem item, int itemNum)
    {
        if (player.item != null)
        {
            CraftingController.instance.DisableItem(player.item, player.itemNum);
        }

        player.item = item;
        player.itemNum = itemNum;
        item.gameObject.transform.position = player.playerGO.transform.position;
        item.gameObject.transform.parent = player.playerGO.transform;
        item.gameObject.SetActive(true);
    }

    public void UseItem()
    {
        item.usesLeft--;
        Debug.Log(item.name + " used 1 time. Number of uses left is " + item.usesLeft);
        item.gameObject.GetComponent<ItemScript>().UseItem(item.type, item.name, this);

        if(item.usesLeft <= 0)
        {
            CraftingController.instance.DisableItem(item, itemNum);
            item = null;
            itemNum = 0;
        }
    }
}