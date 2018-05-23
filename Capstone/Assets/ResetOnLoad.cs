using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ResetOnLoad : MonoBehaviour {

    CanvasGroup canvas;



    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

    }


    // Update is called once per frame
    void Update () {
		
	}
}
