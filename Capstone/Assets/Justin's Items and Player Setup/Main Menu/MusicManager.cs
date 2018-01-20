using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Music
{
    AxhomFissionInstrumental = 0,
    AxhomOrbitInstrumental = 1,
    FeelinGood = 2,
    GetHappy = 3,
    HappyAlley = 4,
    HappyMusic1 = 5,
    LoveLife = 6,
	ZooMusic = 7,
    MainZooMusic = 8,
    MainMenu = 9,
}

public class MusicManager : MonoBehaviour {

    public static MusicManager instance = null;

	public Music music;

    public List<AudioClip> SFXList = new List<AudioClip>();

    private AudioSource auds;

    // Use this for initialization
    void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (this.gameObject);
		}

        if (!GetComponent<AudioSource>())
        {
            auds = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            auds = GetComponent<AudioSource>();
        }

		playMusic (music, 0.5f, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void playMusic(Music music, float volume = 0.5f, bool loop = false)
    {
        auds.volume = volume;
        auds.loop = loop;

        auds.clip = SFXList[(int)music];
        auds.Play();
    }

	public void fadeMusic(float targetVolume = 0.2f, float fadeTime = 1f){
		StartCoroutine (fadeMusicCoroutine (targetVolume, fadeTime));
	}

	private IEnumerator fadeMusicCoroutine(float targetVolume, float fadeTime){
		bool under = true;

		if (targetVolume > auds.volume) {
			under = false;
		}

		while (auds.volume != targetVolume) {
			auds.volume = Mathf.MoveTowards (auds.volume, targetVolume, fadeTime * Time.deltaTime);
			Debug.Log (auds.volume);
			yield return new WaitForEndOfFrame ();

			if (under) {
				if (auds.volume < targetVolume) {
					auds.volume = targetVolume;
				}
			} else {
				if (auds.volume > targetVolume) {
					auds.volume = targetVolume;
				}
			}
		}

		auds.volume = targetVolume;
	}

    public void stopMusic(Music music)
    {
        foreach (AudioSource a in gameObject.GetComponents<AudioSource>())
        {
            if (a.clip == SFXList[(int)music])
            {
                if (a.isPlaying)
                {
                    a.Stop();
                    a.clip = null;
                }
            }
        }
    }
}
