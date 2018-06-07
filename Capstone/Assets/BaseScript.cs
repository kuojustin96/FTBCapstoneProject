using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BaseScript : MonoBehaviour {

    public TextMeshProUGUI[] texts;

    public MeshRenderer baseColorObject;

    public void UpdateColor(Color col)
    {
        baseColorObject.materials[0].color = col;
    }
    public void UpdateNamePlate(string name)
    {
        foreach (TextMeshProUGUI text in texts)
        {
            text.text = "Fort " + name;
        }
    }

}
