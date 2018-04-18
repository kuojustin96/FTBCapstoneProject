using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListName : MonoBehaviour {

    public Text nameText;

    void Start()
    {
        nameText = GetComponent<Text>();
    }

    public void SetName(string name)
    {

        nameText.text = name;

    }

    public void Remove()
    {
        Destroy(gameObject);
    }


}
