using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/Game Settings")]
    public static void CreateMyAsset()
    {
        GameSettingsSO asset = ScriptableObject.CreateInstance<GameSettingsSO>();

        AssetDatabase.CreateAsset(asset, "Assets/GameSettings.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}