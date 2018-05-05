using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jkuo;
using UnityEngine.Networking;

public class JumpPad : NetworkBehaviour {

    public float jumpForce = 50f;
    private Collider col;
    //private Animator animator;
    private NetworkAnimator na;

	// Use this for initialization
	void Start () {
        col = GetComponent<Collider>();
        //animator = GetComponent<Animator>();
        na = GetComponent<NetworkAnimator>();
	}

    private IEnumerator TriggerAnimation()
    {
        na.animator.SetBool("Activate", true);
        col.enabled = false;

        float saveTime = Time.time;
        while (Time.time < saveTime + (na.animator.runtimeAnimatorController.animationClips[0].length + 0.5f))
            yield return null;

        na.animator.SetBool("Activate", false);
        col.enabled = true;
    }

    [Command]
    private void CmdTriggerAnimation()
    {
        RpcTriggerAnimation();
    }

    [ClientRpc]
    private void RpcTriggerAnimation()
    {
        StartCoroutine(TriggerAnimation());
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "NetPlayer")
        {
            CmdTriggerAnimation();
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * (jumpForce * 10), ForceMode.Impulse);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "NetPlayer")
            other.GetComponent<net_PlayerController>().canJump = false;
    }
}
