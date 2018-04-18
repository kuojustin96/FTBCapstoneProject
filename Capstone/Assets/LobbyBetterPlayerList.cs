using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBetterPlayerList : MonoBehaviour {

    public GameObject nameObject;

    public LobbyListName CreateName(string name)
    {

        GameObject obj = Instantiate(nameObject, transform);

        LobbyListName theListName = obj.GetComponent<LobbyListName>();

        Debug.Assert(theListName);

        theListName.SetName(name);
        theListName.gameObject.name = name;

        return theListName;
    }



}
