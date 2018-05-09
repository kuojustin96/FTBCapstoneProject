using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jkuo;
//using UnityEngine.Networking;

public class JumpPad : MonoBehaviour {

    public float jumpForce = 50f;
   // private Collider col;
    //private Animator animator;
	public Animator na;

	// Use this for initialization
	void Start () {
       // col = GetComponent<Collider>();
        //animator = GetComponent<Animator>();
       // na = GetComponent<Animator>();
	}

	public IEnumerator TriggerAnimation(GameObject other)
    {
		yield return new WaitForSeconds (.15f);
		other.GetComponent<Rigidbody>().AddForce(Vector3.up * (jumpForce * 10), ForceMode.Impulse);
        na.SetBool("Activate", true);
		GetComponent<Collider>().enabled = false;

        float saveTime = Time.time;
        while (Time.time < saveTime + (na.runtimeAnimatorController.animationClips[0].length + 0.5f))
            yield return null;

        na.SetBool("Activate", false);
		GetComponent<Collider>().enabled = true;
    }

//    [Command]
//    private void CmdTriggerAnimation()
//    {
//        RpcTriggerAnimation();
//    }
//
//    [ClientRpc]
//    private void RpcTriggerAnimation()
//    {
//        StartCoroutine(TriggerAnimation());
//    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "NetPlayer")
        {
            //CmdTriggerAnimation();
            
			StartCoroutine(TriggerAnimation(other.gameObject));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "NetPlayer")
            other.GetComponent<net_PlayerController>().canJump = false;
    }
}
