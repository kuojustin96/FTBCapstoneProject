using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class QuickTestPlayEditor : EditorWindow {

    string lobbyScenePath;

    public SceneAsset lobbyScene;
    public SceneAsset playScene;
    public SceneAsset winScene;

    //todo: serialize lobbyScene
    [MenuItem("Test/Test play window")]
    static void Init()
    {
        QuickTestPlayEditor window = (QuickTestPlayEditor)EditorWindow.GetWindow(typeof(QuickTestPlayEditor));
        window.Show();
    }

    void OnGUI()
    {
        lobbyScene = EditorGUILayout.ObjectField("Lobby Scene", lobbyScene, typeof(SceneAsset), false) as SceneAsset;

        if (lobbyScene)
        {
            if(GUILayout.Button("Swap To Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(lobbyScene));
            }

            if (GUILayout.Button("Test"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(lobbyScene));
                EditorApplication.isPlaying = true;
            }
        }

        EditorGUILayout.Space();

        playScene = EditorGUILayout.ObjectField("Play Scene", playScene, typeof(SceneAsset), false) as SceneAsset;

        if (playScene)
        {
            if (GUILayout.Button("Swap To Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(playScene));
            }
        }

        EditorGUILayout.Space();

        winScene = EditorGUILayout.ObjectField("Win Scene", winScene, typeof(SceneAsset), false) as SceneAsset;

        if (winScene)
        {
            if (GUILayout.Button("Swap To Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(winScene));
            }
        }

    }



}
