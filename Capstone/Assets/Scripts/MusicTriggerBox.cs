using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerBox : MonoBehaviour {

    private MusicManager mm;
    public string mainTrackName;
    [Tooltip("Put value of -1 to use default fade time")]
    public float fadeTime = -1f;
    public float targetVolume = 1f;

	// Use this for initialization
	void Start () {
        mm = MusicManager.instance;

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
                if(fadeTime < 0)
                    mm.SwapMainTracks(mainTrackName, targetVolume, mm.defaultFadeTime);
                else
                    mm.SwapMainTracks(mainTrackName, targetVolume, fadeTime);
            }

            mm.currentMusicTrigger = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (mm.currentMusicTrigger != null)
        {
            if (mm.currentMusicTrigger != this)
            {
                mm.SwapMainTracks(mainTrackName, targetVolume, mm.defaultFadeTime);
            }
            else
            {
                mm.SwapMainTracks(mainTrackName, targetVolume, mm.defaultFadeTime);

                mm.currentMusicTrigger = null;
            }
        }
        else
        {
            //This should never be called
            mm.SwapMainTracks(mm.defaultTrackName, 1f, mm.defaultFadeTime);
        }
    }
}
