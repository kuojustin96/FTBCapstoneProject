using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugDisplayScript : MonoBehaviour {

    public NetworkProfile profile;
    public TextMeshProUGUI text;
	// Use this for initialization
	void Start () {
        text = GetComponent<TextMeshProUGUI>();	
	}
	
	// Update is called once per frame
	void Update () {
        text.text = profile.outfitChoice.ToString();
	}
}
