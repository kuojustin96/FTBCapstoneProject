using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_FallCalculation : MonoBehaviour {

	Rigidbody rb;
	public float fallThreshold = 3f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		float downVel = Mathf.Abs(rb.velocity.y);
		Debug.Log("downVel " + downVel);
		if(downVel > fallThreshold)
		{
			//Fall stun duration calculation
			float rbVelHalf = downVel / 2;
			float x = 1 + (downVel - fallThreshold);
			float stunDur = (rbVelHalf + (downVel / x)) - fallThreshold;
			Debug.Log(stunDur);
		}
	}
}
