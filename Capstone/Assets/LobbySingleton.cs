using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LobbySingleton : MonoBehaviour
{


    public static LobbySingleton instance = null;

    [SerializeField]
    GameObject readyUpText;

    [SerializeField]
    CinemachineVirtualCamera lobbyCam;

    [SerializeField]
    CinemachineVirtualCamera playerCam;

    [SerializeField]
    CinemachineVirtualCamera transitionCam;

    [SerializeField]
    CanvasGroup fader;

    public float fadeTime = 1.0f;

    public CinemachineVirtualCamera TransitionCam
    {
        get
        {
            return transitionCam;
        }

        set
        {
            transitionCam = value;
        }
    }

    public CinemachineVirtualCamera PlayerCam
    {
        get
        {
            return playerCam;
        }

        set
        {
            playerCam = value;
        }
    }

    public CinemachineVirtualCamera LobbyCam
    {
        get
        {
            return lobbyCam;
        }

        set
        {
            lobbyCam = value;
        }
    }

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
        Debug.Log("haha");
        return readyUpText;
    }


    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

    }


    public void FadeIn()
    {
        FadeManager.instance.FadeIn(Fader, fadeTime);


    }

}
