using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyUpManager : MonoBehaviour
{


    public static ReadyUpManager instance = null;

    [SerializeField]
    GameObject readyUpText;


    [SerializeField]
    CanvasGroup fader;

    public float fadeTime = 1.0f;



    public CanvasGroup Fader
    {
        get
        {
            return fader;
        }

        set
        {
            fader = value;
        }
    }

    public GameObject getReadyUpText()
    {
        //Debug.Log("haha");
        return readyUpText;
    }


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }


    public void FadeIn()
    {
        FadeManager.instance.FadeIn(Fader, fadeTime);


    }

}
