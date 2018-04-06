using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour {

    public static SoundEffectManager instance = null;

    //[HideInInspector]
    public Object[] SFXList;

    private Dictionary<string, AudioClip> SFXDict = new Dictionary<string, AudioClip>();

	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        LoadSoundEffects();
    }

    public void LoadSoundEffects()
    {
        SFXList = Resources.LoadAll("Sound Effects", typeof(AudioClip));

        foreach (Object o in SFXList)
            SFXDict.Add(o.name, (AudioClip)o);

        //Debug Code
        //foreach (string s in SFXDict.Keys)
        //    Debug.Log(s);

        //foreach (AudioClip ac in SFXDict.Values)
        //    Debug.Log(ac.name);
    }

    public void PlaySFX(string sfxName, float volume = 1f)
    {
        AudioSource auds = gameObject.AddComponent<AudioSource>();
        auds.clip = SFXDict[sfxName];
        auds.volume = volume;
        auds.Play();

        StartCoroutine(c_StopSFX(auds));
    }

    private IEnumerator c_StopSFX(AudioSource auds)
    {
        float startTime = Time.time;
        while (Time.time < startTime + auds.clip.length)
            yield return null;

        auds.Stop();
        Destroy(auds);
    }
}
