using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkParticleController : NetworkBehaviour {

    private ObjectPoolManager opm;

	// Use this for initialization
	void Start () {
        opm = ObjectPoolManager.instance;
	}

    [Command]
    public void CmdPlayParticleEffect(string name, GameObject parent, Vector3 position, float size)
    {
        RpcPlayParticleEffect(name, parent, position, size);
    }

    [ClientRpc]
    public void RpcPlayParticleEffect(string name, GameObject parent, Vector3 position, float size)
    {
        StartCoroutine(PlayParticleEffect(name, parent, position, size));
    }

    private IEnumerator PlayParticleEffect(string name, GameObject parent, Vector3 position, float size)
    {
        GameObject g = ObjectPoolManager.instance.SpawnObject(name, size);
        float duration = g.GetComponent<ParticleSystem>().main.duration;
        g.transform.parent = parent.transform;
        g.transform.position = position;

        float saveTime = Time.time;
        while (Time.time < saveTime + duration)
            yield return null;

        ObjectPoolManager.instance.RecycleObject(name, g);
    }
}
