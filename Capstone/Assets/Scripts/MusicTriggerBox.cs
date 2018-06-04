using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerBox : MonoBehaviour {

    private MusicManager mm;
    public string mainTrackName;
    [Tooltip("Put value of -1 to use default fade time")]
    public float fadeTime = -1f;
    public float targetVolume = 1f;

    AudioSource temp;

	// Use this for initialization
	void Start () {
        //mm = MusicManager.instance;
        mm = MusicManager.instance;
        //temp = GameObject.Find("PlayerClassController").GetComponent<AudioSource>();

        //Safety function
        GetComponent<Collider>().isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
	}
	
    //OnTriggerEnter has a higher priority call than OnTriggerExit
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NetPlayer" || other.tag == "LobbyPlayer")
        {
            if (mm.currentMusicTrigger == null)
            {
                AudioSource temp = other.transform.root.GetComponent<LocalMusicManager>().mainTrackAuds;

                if(fadeTime < 0)
                    mm.SwapMainTracks(mainTrackName, targetVolume, mm.defaultFadeTime, temp);
                else
                    mm.SwapMainTracks(mainTrackName, targetVolume, fadeTime, temp);
            }

            mm.currentMusicTrigger = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!GameManager.instance.endGame)
        {
            if (mm.currentMusicTrigger != null)
            {
                AudioSource temp = other.transform.root.GetComponent<LocalMusicManager>().mainTrackAuds;

                if (mm.currentMusicTrigger != this)
                {
                    mm.SwapMainTracks(mainTrackName, targetVolume, mm.defaultFadeTime, temp);
                }
                else
                {
                    mm.SwapMainTracks(mainTrackName, targetVolume, mm.defaultFadeTime, temp);

                    mm.currentMusicTrigger = null;
                }
            }
            else
            {
                AudioSource temp = other.transform.root.GetComponent<LocalMusicManager>().mainTrackAuds;
                mm.SwapMainTracks(mm.defaultGameplayTrack, 1f, mm.defaultFadeTime, temp);
            }
        }
    }
}
