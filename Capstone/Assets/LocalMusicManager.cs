using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMusicManager : MonoBehaviour {

    private MusicManager mm;
    public AudioSource mainTrackAuds { get; protected set; }

	// Use this for initialization
	void Awake () {
        mm = MusicManager.instance;

        if (!GetComponent<AudioSource>())
            mainTrackAuds = gameObject.AddComponent<AudioSource>();
        else
            mainTrackAuds = GetComponent<AudioSource>();

        mainTrackAuds.loop = true;
        mainTrackAuds.playOnAwake = false;
        mainTrackAuds.outputAudioMixerGroup = mm.audMixer.FindMatchingGroups("Music")[0];
        mainTrackAuds.spatialBlend = 1f;
        mainTrackAuds.maxDistance = 1f;
    }

    public void SwapMainTracks(string newMainTrackName, float targetVol, float fadeTime, AudioSource targetSource)
    {
        mm.SwapMainTracks(newMainTrackName, targetVol, fadeTime, targetSource);
    }
}
