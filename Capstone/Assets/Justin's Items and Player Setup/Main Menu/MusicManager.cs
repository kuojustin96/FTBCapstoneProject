using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DG.Tweening;

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

    public AudioMixer audMixer;
    [HideInInspector]
    public MusicTriggerBox currentMusicTrigger;
    public float defaultFadeTime = 1f;
    public string defaultTrackName;

    public MusicTracks[] Music;

    private AudioSource mainTrackAuds;

    // Use this for initialization
    void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this.gameObject);
		}
			DontDestroyOnLoad (gameObject);

        DOTween.Init();
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (!GetComponent<AudioSource>())
            mainTrackAuds = gameObject.AddComponent<AudioSource>();
        else
            mainTrackAuds = GetComponent<AudioSource>();

        LoadMusicLibrary();

        PlayMainTrack(defaultTrackName);
    }

    void Start()
    {
        mainTrackAuds.loop = true;
        mainTrackAuds.playOnAwake = false;
        mainTrackAuds.outputAudioMixerGroup = audMixer.FindMatchingGroups("Music")[0];
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
            SwapMainTracks("GroundFloor", 1f, 1f);
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
                {
                    if (!m.musicComponents.ContainsKey("MainTrack"))
                        m.musicComponents.Add("MainTrack", (AudioClip)m.musicList[x]);
                    else
                        m.musicComponents["MainTrack"] = (AudioClip)m.musicList[x];
                }
            }
        }
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

        Debug.Log("DIDN'T FIND THE SONG");
    }

    public void StopMainTrack(float fadeTime)
    {
        if (fadeTime != 0)
            StartCoroutine(c_FadeMainTrack(0, fadeTime, false));
        else
            mainTrackAuds.Stop();
    }

    private IEnumerator c_FadeMainTrack(float targetVol, float fadeTime, bool playMusic)
    {
        if (playMusic)
            mainTrackAuds.Play();

        float counter = 0;
        while(counter < fadeTime)
        {
            mainTrackAuds.DOFade(targetVol, fadeTime).SetEase(Ease.Linear);
            counter += Time.deltaTime / fadeTime;
            yield return null;
        }

        mainTrackAuds.volume = targetVol;

        if (!playMusic)
            mainTrackAuds.Stop();
    }

    public void SwapMainTracks(string newMainTrackName, float targetVol, float fadeTime)
    {
        foreach(MusicTracks m in Music)
        {
            if(newMainTrackName == m.musicName)
            {
                StartCoroutine(c_SwapMainTracks(m.musicComponents["MainTrack"], targetVol, fadeTime));
                return;
            }
        }
    }

    private IEnumerator c_SwapMainTracks(AudioClip newMainTrack, float targetVol, float fadeTime)
    {
        //Check if there actually is a main track playing
        if (mainTrackAuds.clip != null)
        {
            StartCoroutine(c_FadeMainTrack(0, fadeTime / 2, false));

            float saveTime = Time.time;
            while (Time.time < saveTime + (fadeTime / 2))
                yield return null;

        }

        mainTrackAuds.clip = newMainTrack;

        StartCoroutine(c_FadeMainTrack(targetVol, fadeTime, true));
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
            //auds.volume = Mathf.MoveTowards(auds.volume, mainTrackAuds.volume, Time.deltaTime * fadeTime);
            auds.DOFade(auds.volume, fadeTime).SetEase(Ease.Linear);
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
            //auds.volume = Mathf.MoveTowards(auds.volume, 0, Time.deltaTime * fadeTime);
            auds.DOFade(auds.volume, fadeTime).SetEase(Ease.Linear);
            yield return null;
        }

        auds.volume = 0;
        musicTrack.componentsPlaying.Remove(componentName);
        Destroy(auds);
    }
}