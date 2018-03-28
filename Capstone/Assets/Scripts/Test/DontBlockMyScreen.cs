using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DontBlockMyScreen : MonoBehaviour {

//Enables in play mode


    void Awake()
    {
        GetComponent<RawImage>().enabled = true;
    }

}
