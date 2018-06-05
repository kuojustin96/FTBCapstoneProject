using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class tornadoTrigger : NetworkBehaviour {
	public float radius = 5.0F;
	public float power = 10.0F;
	public GameObject parentPlayer;
    private NetworkSoundController nsc;

    void OnEnable()
	{
        nsc = GetComponent<NetworkSoundController>();
        nsc.CmdPlaySFX("Tornado", gameObject, 1f, 400f, true, true);
		Destroy (gameObject, 30);
	}
	void FixedUpdate(){
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			if (hit.tag == "NetPlayer") {
				Rigidbody rb = hit.GetComponent<Rigidbody> ();

				if (rb != null && rb.gameObject != parentPlayer)
					rb.AddExplosionForce (power, explosionPos, radius);
			}
		}
	}

    private void OnDestroy()
    {
        nsc.CmdStopSFX("Tornado", gameObject);
    }
}