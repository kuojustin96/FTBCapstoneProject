using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class QuickTestPlayEditor : EditorWindow {

    string lobbyScenePath;
    public SceneAsset lobbyScene;

    //todo: serialize lobbyScene
    [MenuItem("Test/Test play window")]
    static void Init()
    {
        QuickTestPlayEditor window = (QuickTestPlayEditor)EditorWindow.GetWindow(typeof(QuickTestPlayEditor));
        window.Show();
    }

    void OnGUI()
    {

        lobbyScene = EditorGUILayout.ObjectField("Scene", lobbyScene, typeof(SceneAsset), false) as SceneAsset;

        if (lobbyScene)
        {
            if (GUILayout.Button("Test"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(lobbyScene));
                EditorApplication.isPlaying = true;
            }
        }
    }

}
