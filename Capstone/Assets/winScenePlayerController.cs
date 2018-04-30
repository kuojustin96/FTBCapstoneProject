using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class winScenePlayerController : NetworkBehaviour {

    public ParticleSystem[] Emotes;
    private bool playingEmote = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        UseEmotes();
	}

    private void UseEmotes()
    {
        if (!playingEmote)
        {
            if (!isLocalPlayer)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                CmdEmote(0);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                CmdEmote(1);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                CmdEmote(2);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                CmdEmote(3);
        }
    }

    [Command]
    private void CmdEmote(int emoteNum)
    {
        Debug.Log("CALLED EMOTE COMMAND");
        RpcEmote(emoteNum);
    }

    [ClientRpc]
    private void RpcEmote(int emoteNum)
    {
        Emotes[emoteNum].Play();
        Debug.Log("PLAYING EMOTE");
        playingEmote = true;

        if (isLocalPlayer)
        {
            Vector3 temp = Emotes[emoteNum].transform.localPosition;
            temp.x = -4;
            Emotes[emoteNum].transform.localPosition = temp;
            Emotes[emoteNum].GetComponent<ParticleSystemRenderer>().alignment = ParticleSystemRenderSpace.View;
            StartCoroutine(c_EmoteCooldown(emoteNum));
        }
    }

    private IEnumerator c_EmoteCooldown(int emoteNum)
    {
        float saveTime = Time.time;
        float psDuration = Emotes[emoteNum].main.duration;
        while (Time.time < saveTime + psDuration)
            yield return null;

        playingEmote = false;
    }
}
