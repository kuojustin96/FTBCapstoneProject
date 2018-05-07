using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerGameProfile : MonoBehaviour {

    public static PlayerGameProfile profile = null;

    public TMP_InputField text;

    public string playerName;


    public void UpdatePlayerName()
    {
        Debug.Log("Name is " + text.text);
        playerName = text.text;

        PlayerPrefs.SetString("PlayerName", playerName);
    }



	// Use this for initialization
	void Start () {

        string theName = PlayerPrefs.GetString("PlayerName");
        if (PlayerPrefs.GetString("PlayerName") == "")
        {
            playerName = "Chad";
        }
        else
        {
            playerName = theName;
        }
        //updates the name field;
        text.text = playerName;
    }

}
