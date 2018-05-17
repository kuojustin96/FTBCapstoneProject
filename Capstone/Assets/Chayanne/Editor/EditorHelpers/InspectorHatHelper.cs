using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkOutfitScript))]
public class InspectorHatHelper : Editor {


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CreatePopulateButton();

    }


    void CreatePopulateButton()
    {
        NetworkOutfitScript script = (NetworkOutfitScript)target;

        if (script.HatRoot && GUILayout.Button("Find Hats"))
        {
            if (script)
            {
                script.PopulateHats();
            }
            else
            {
                Debug.LogError("Something went wrong");
            }
        }

    }
}
