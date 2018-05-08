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
    public void CmdPlaySFX(string sfxName, GameObject audioObject)
    {
        RpcPlaySFX(sfxName, audioObject);
    }

    [ClientRpc]
    public void RpcPlaySFX(string sfxName, GameObject audioObject)
    {
        sfm.PlaySFX(sfxName, audioObject);
    }

    [Command]
    public void CmdStopSFX(string sfxName)
    {
        RpcStopSFX(sfxName);
    }

    [ClientRpc]
    public void RpcStopSFX(string sfxName)
    {
        sfm.StopSFX(sfxName);
    }
}
