using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class StreamVideo : MonoBehaviour {

    public VideoClip startClip;
    public VideoClip endClip;
    private VideoPlayer videoPlayer;
    private RawImage rawImage;

	// Use this for initialization
	void Start () {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();

        videoPlayer.playOnAwake = false;
	}

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //    PlayStartClip();
    }

    public void PlayStartClip()
    {
        videoPlayer.clip = startClip;
        videoPlayer.Play();
        Debug.Log(videoPlayer.isPlaying + "DJISADJASODIO");
    }

    public void PlayEndClip()
    {

    }
}
