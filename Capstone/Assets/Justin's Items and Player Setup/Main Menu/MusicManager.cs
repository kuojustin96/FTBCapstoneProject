﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance = null;

    [System.Serializable]
    public class MusicTracks
    {
        public string musicName;
        [HideInInspector] public Object[] musicList;
        public Dictionary<string, AudioClip> musicComponents = new Dictionary<string, AudioClip>();
        public Dictionary<string, AudioSource> componentsPlaying = new Dictionary<string, AudioSource>();
    }

    public MusicTracks[] Music;

    private AudioSource mainTrackAuds;

    // Use this for initialization
    void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (this.gameObject);
		}

        if (!GetComponent<AudioSource>())
            mainTrackAuds = gameObject.AddComponent<AudioSource>();
        else
            mainTrackAuds = GetComponent<AudioSource>();

        LoadMusicLibrary();

		//playMusic (music, 0.5f, true);
    }

    void Start()
    {
        mainTrackAuds.loop = true;
        mainTrackAuds.playOnAwake = false;
    }

    private void LoadMusicLibrary()
    {
        foreach(MusicTracks m in Music)
        {
            m.musicList = Resources.LoadAll("Music/" + m.musicName, typeof(AudioClip));

            for (int x = 0; x < m.musicList.Length; x++)
            {
                string[] splitString = m.musicList[x].name.Split('(', ')');
                if (splitString.Length > 1)
                    m.musicComponents.Add(splitString[1], (AudioClip)m.musicList[x]);
                else
                    m.musicComponents.Add("MainTrack", (AudioClip)m.musicList[x]);
            }
        }

        //Debug Code
        //foreach (string s in Music[0].musicComponents.Keys)
        //    Debug.Log(s);

        //foreach (AudioClip ac in Music[0].musicComponents.Values)
        //    Debug.Log(ac.name);
    }


    public void PlayMainTrack(string mainTrackName, float fadeTime = 0, float volume = 1f)
    {
        foreach(MusicTracks m in Music)
        {
            if(m.musicName == mainTrackName)
            {
                mainTrackAuds.clip = m.musicComponents["MainTrack"];

                if (fadeTime != 0)
                {
                    StartCoroutine(c_FadeMainTrack(volume, fadeTime, true));
                }
                else
                {
                    mainTrackAuds.volume = volume;
                    mainTrackAuds.Play();
                }

                return;
            }
        }
    }

    public void StopMainTrack(string mainTrackName, float fadeTime)
    {
        foreach (MusicTracks m in Music)
        {
            if (m.musicName == mainTrackName)
            {
                if (fadeTime != 0)
                    StartCoroutine(c_FadeMainTrack(0, fadeTime, false));
                else
                    mainTrackAuds.Stop();

                return;
            }
        }
    }

    private IEnumerator c_FadeMainTrack(float targetVol, float fadeTime, bool playMusic)
    {
        if (playMusic)
            mainTrackAuds.Play();

        float counter = 0;
        while(counter < fadeTime)
        {
            mainTrackAuds.volume = Mathf.MoveTowards(mainTrackAuds.volume, targetVol, Time.deltaTime * fadeTime);
            counter += Time.deltaTime / fadeTime;
            yield return null;
        }

        mainTrackAuds.volume = targetVol;

        if (!playMusic)
            mainTrackAuds.Stop();
    }


    public void FadeInMusicComponent(string mainTrackName, string componentName, float fadeTime = 1f)
    {
        foreach(MusicTracks m in Music)
        {
            if (m.musicName == mainTrackName)
            {
                StartCoroutine(c_FadeInMusicComponent(m, componentName, fadeTime));
                return;
            }
        }
    }

    private IEnumerator c_FadeInMusicComponent(MusicTracks musicTrack, string componentName, float fadeTime)
    {
        AudioSource auds = gameObject.AddComponent<AudioSource>();
        musicTrack.componentsPlaying.Add(componentName, auds);
        auds.clip = musicTrack.musicComponents[componentName];
        auds.timeSamples = mainTrackAuds.timeSamples;
        auds.volume = 0f;
        auds.loop = true;
        auds.playOnAwake = false;
        auds.Play();

        while(auds.volume < mainTrackAuds.volume)
        {
            auds.volume = Mathf.MoveTowards(auds.volume, mainTrackAuds.volume, Time.deltaTime * fadeTime);
            yield return null;
        }

        auds.volume = mainTrackAuds.volume;
    }

    public void FadeOutMusicComponent(string mainTrackName, string componentName, float fadeTime = 1f)
    {
        foreach (MusicTracks m in Music)
        {
            if (m.musicName == mainTrackName)
            {
                StartCoroutine(c_FadeOutMusicComponent(m, componentName, fadeTime));
                return;
            }
        }
    }

    private IEnumerator c_FadeOutMusicComponent(MusicTracks musicTrack, string componentName, float fadeTime)
    {
        AudioSource auds = musicTrack.componentsPlaying[componentName];

        while (auds.volume > 0)
        {
            auds.volume = Mathf.MoveTowards(auds.volume, 0, Time.deltaTime * fadeTime);
            yield return null;
        }

        auds.volume = 0;
        musicTrack.componentsPlaying.Remove(componentName);
        Destroy(auds);
    }
}