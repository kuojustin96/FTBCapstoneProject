using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectManager : MonoBehaviour {

    public static SoundEffectManager instance = null;
    public AudioMixer audMixer;

    [System.Serializable]
    public class SoundEffectCategories
    {
        public string name;
        public List<AudioClip> SFXList = new List<AudioClip>();

        public AudioClip GetRandomClip()
        {
            int rand = Random.Range(0, SFXList.Count);
            return SFXList[rand];
        }
    }

    public SoundEffectCategories[] sfxCats;

    private Dictionary<string, AudioSource> SFXPlaying = new Dictionary<string, AudioSource>();

	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        LoadSoundEffects();
    }

    private void LoadSoundEffects()
    {
        Object[] temp;
        foreach (SoundEffectCategories seo in sfxCats)
        {
            temp = Resources.LoadAll("Sound Effects/" + seo.name, typeof(AudioClip));
            foreach (AudioClip ac in temp)
                seo.SFXList.Add(ac);
        }

    }

    public void PlaySFX(string sfxTypeName, GameObject audioObject, float volume = 1f, bool canHaveMultiple = false)
    {
        if (!canHaveMultiple)
        {
            if (SFXPlaying.ContainsKey(sfxTypeName))
                return;
        }

        AudioSource auds = null;

        foreach(SoundEffectCategories seo in sfxCats)
        {
            if(sfxTypeName == seo.name)
            {
                if (audioObject)
                {
                    auds = audioObject.AddComponent<AudioSource>();
                    auds.clip = seo.GetRandomClip();
                }
                break;
            }
        }

        if (auds)
        {
            DefaultAudioSettings(auds);

            auds.volume = volume;
            auds.Play();

            if (!canHaveMultiple)
            {
                if (!SFXPlaying.ContainsKey(sfxTypeName))
                    SFXPlaying.Add(sfxTypeName, auds);
            }

            StartCoroutine(c_StopSFX(auds, sfxTypeName));
        }
    }

    private void DefaultAudioSettings(AudioSource aud)
    {
        aud.loop = false;
        aud.playOnAwake = false;
        aud.outputAudioMixerGroup = audMixer.FindMatchingGroups("Sound Effects")[0];
        aud.spatialBlend = 1f;
        aud.rolloffMode = AudioRolloffMode.Linear;
        aud.maxDistance = 100f;
    }

    //Only works if canHaveMultiple was set to false
    public void StopSFX(string sfxTypeName)
    {
        if (SFXPlaying.ContainsKey(sfxTypeName))
        {
            AudioSource temp = SFXPlaying[sfxTypeName];
            temp.Stop();
            SFXPlaying.Remove(sfxTypeName);
            Destroy(temp);
        }
    }

    private IEnumerator c_StopSFX(AudioSource auds, string sfxTypeName)
    {
        float startTime = Time.time;
        float clipLength = auds.clip.length;
        while (Time.time < startTime + clipLength)
        {
            if (auds == null)
                break;

            yield return null;
        }

        if (auds)
        {
            auds.Stop();

            if (SFXPlaying.ContainsKey(sfxTypeName))
                SFXPlaying.Remove(sfxTypeName);

            Destroy(auds);
        }
    }

    public void StopAllSFX()
    {
        foreach (KeyValuePair<string, AudioSource> pair in SFXPlaying)
        {
            if (pair.Value != null)
            {
                SFXPlaying[pair.Key].Stop();
                Destroy(SFXPlaying[pair.Key]);
                SFXPlaying.Remove(pair.Key);
            }
        }
    }
}
