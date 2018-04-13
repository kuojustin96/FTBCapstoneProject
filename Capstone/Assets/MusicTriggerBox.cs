using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerBox : MonoBehaviour {

    private MusicManager mm;
    public string mainTrackName;

	// Use this for initialization
	void Start () {
        mm = MusicManager.instance;

        //Safety function
        GetComponent<Collider>().isTrigger = true;
	}
	
    //OnTriggerEnter has a higher priority call than OnTriggerExit
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NetPlayer")
        {
            if (mm.currentMusicTrigger == null)
            {
                mm.SwapMainTracks(mainTrackName, 1f, mm.defaultFadeTime);
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
                mm.SwapMainTracks(mm.currentMusicTrigger.mainTrackName, 1f, mm.defaultFadeTime);
            }
            else
            {
                mm.SwapMainTracks(mm.defaultTrackName, 1f, mm.defaultFadeTime);
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
