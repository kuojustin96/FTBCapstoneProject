using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSoundController : NetworkBehaviour {

    private SoundEffectManager sfm;

	// Use this for initialization
	void Start () {
        sfm = SoundEffectManager.instance;
	}

    [Command]
    public void CmdPlaySFX(string sfxName, GameObject audioObject, float volume, float maxDistance, bool canHaveMultiple, bool looping)
    {
        RpcPlaySFX(sfxName, audioObject, volume, maxDistance, canHaveMultiple, looping);
    }

    [ClientRpc]
    public void RpcPlaySFX(string sfxName, GameObject audioObject, float volume, float maxDistance, bool canHaveMultiple, bool looping)
    {
        sfm.PlaySFX(sfxName, audioObject, volume, maxDistance, canHaveMultiple, looping);
    }

    public void PlaySFXLocal(string sfxName, GameObject audioObject, float volume, float maxDistance, bool canHaveMultiple, bool looping)
    {
        sfm.PlaySFX(sfxName, audioObject, volume, maxDistance, canHaveMultiple, looping);
    }


    [Command]
    public void CmdStopSFX(string sfxName, GameObject audioObject)
    {
        RpcStopSFX(sfxName, audioObject);
    }

    public void StopSFXLocal(string sfxName, GameObject audioObject)
    {
        sfm.StopSFX(sfxName, audioObject);
    }

    [ClientRpc]
    public void RpcStopSFX(string sfxName, GameObject audioObject)
    {
        sfm.StopSFX(sfxName, audioObject);
    }

    [Command]
    public void CmdStopAllSFX()
    {
        RpcStopAllSFX();
    }

    [ClientRpc]
    public void RpcStopAllSFX()
    {
        sfm.StopAllSFX();
    }
}
