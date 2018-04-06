using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jkuo;

public class JumpPad : MonoBehaviour {

    public float jumpForce = 50f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "NetPlayer")
        {
            //Change this so that it just calls a single function
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * (jumpForce * 10), ForceMode.Impulse);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "NetPlayer")
            other.GetComponent<net_PlayerController>().canJump = false;
    }
}
