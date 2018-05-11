using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TempAudioTest : MonoBehaviour {

    public AudioClip audClip;
    private AudioSource mainTrackAuds;
    public AudioMixer audMixer;

    // Use this for initialization
    void Start () {

        if (!GetComponent<AudioSource>())
            mainTrackAuds = gameObject.AddComponent<AudioSource>();
        else
            mainTrackAuds = GetComponent<AudioSource>();

        mainTrackAuds.loop = true;
        mainTrackAuds.playOnAwake = false;
        mainTrackAuds.outputAudioMixerGroup = audMixer.FindMatchingGroups("Music")[0];

        mainTrackAuds.clip = audClip;
        mainTrackAuds.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
