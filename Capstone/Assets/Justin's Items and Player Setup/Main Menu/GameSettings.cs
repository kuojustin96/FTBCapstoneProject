using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSettings : MonoBehaviour {

    public static GameSettings instance = null;
    public TMP_Dropdown playerNumDropdown;

    [HideInInspector]
    public int numPlayers = 2;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetNumPlayers()
    {
        int x = playerNumDropdown.value;
        numPlayers = int.Parse(playerNumDropdown.options[x].text);
    }
}
